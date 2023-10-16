using Knowledge4e.Core.Attributes;
using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Enums;
using Knowledge4e.Core.Extensions;
using Knowledge4e.Core.Helpers;
using Knowledge4e.Infarstructure.Repositories.BaseRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Knowledge4e.Core.Services
{
    /// <summary>
    /// Service dùng chung
    /// </summary>
    /// <typeparam name="TEntity">Loại thực thể</typeparam>
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        #region Declare
        IBaseRepository<TEntity> _baseRepository;
        protected ServiceResult _serviceResult;
        public Type _modelType;
        protected string _tableName = "";
        #endregion

        #region Constructer
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName().ToLowerInvariant();
            _serviceResult = new ServiceResult()
            {
                Code = Enums.Enums.Success,
                Messasge = Properties.Resources.Msg_Success,
            };
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        public async Task<IEnumerable<TEntity>> GetEntities()
        {
            var entities = await _baseRepository.GetEntities();
            return entities.ToList();
        }

        /// <summary>
        /// Custom filter
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        public async Task<ServiceResult> GetEntitiesFilter(PagingRequest pagingRequest, string viewOrTableName = "")
        {
            viewOrTableName = CustomTableNameService(viewOrTableName);
            StringBuilder stringBuilder = new StringBuilder();
            var filter = JsonConvert.DeserializeObject<JArray>(FunctionHelper.Base64Decode(pagingRequest.Filter));
            List<string> columns = new();

            if (filter != null && filter.Type == JTokenType.Array)
            {
                BuildFilterClause(ref stringBuilder, filter);
            }
            else
            {
                stringBuilder.Append(" 1 = 1 ");
            }

            int totalRecord = await _baseRepository.CountTotalRecordByClause(stringBuilder.ToString(), viewOrTableName);


            if (!string.IsNullOrEmpty(pagingRequest.Sort))
            {
                var sort = JsonConvert.DeserializeObject<JArray>(FunctionHelper.Base64Decode(pagingRequest.Sort));
                if (sort != null && sort.Type == JTokenType.Array)
                {
                    stringBuilder.Append(" ORDER BY ");
                    var listSort = new List<string>();

                    foreach (var item in sort)
                    {
                        if (item.Type == JTokenType.Array && item.Count() >= 2)
                        {
                            string name = item[0].Value<string>().Trim();
                            string sortType = item[1].Value<string>().Trim();
                            if (!string.IsNullOrEmpty(name) && (sortType == SortType.ASC || sortType == SortType.DESC))
                            {
                                listSort.Add($"`{name}` {sortType}");
                            }
                        }
                    }

                    stringBuilder.Append(string.Join(", ", listSort));
                }

            }

            if (pagingRequest.PageIndex > 0 && pagingRequest.PageSize > 0)
            {
                stringBuilder.Append($" LIMIT {pagingRequest.PageSize} OFFSET {pagingRequest.PageSize * (pagingRequest.PageIndex - 1) + pagingRequest.Delta}");
            }

            if (!string.IsNullOrEmpty(pagingRequest.Columns))
            {
                columns = pagingRequest.Columns.Split(",".ToCharArray()).Select(item => "`" + item.Trim() + "`").ToList();
            }

            if (totalRecord > 0)
            {
                string cols = !columns.Any() ? "*" : string.Join(", ", columns);
                var data = await _baseRepository.GetEntitiesFilter(stringBuilder.ToString(), cols, viewOrTableName);

                _serviceResult.Data = new
                {
                    totalRecord,
                    totalPage = totalRecord % pagingRequest.PageSize == 0 ? totalRecord / pagingRequest.PageSize : totalRecord / pagingRequest.PageSize + 1,
                    pageSize = pagingRequest.PageSize,
                    pageNumber = pagingRequest.PageIndex,
                    pageData = data
                };
            }
            else
            {
                _serviceResult.Data = new
                {
                    TotalRecord = totalRecord,
                    TotalPage = 0,
                    pagingRequest.PageSize,
                    PageNumber = pagingRequest.PageIndex,
                    PageData = new List<TEntity>()
                };
            }

            return _serviceResult;
        }

        /// <summary>
        /// Allow to custom table name from derived class
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual string CustomTableNameService(string tableName)
        {
            return tableName;
        }

        /// <summary>
        /// Đếm tổng record
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="viewOrTableName"></param>
        /// <returns></returns>
        public async Task<int> CountTotalRecordByClause(PagingRequest pagingRequest, string viewOrTableName = "")
        {
            viewOrTableName = CustomTableNameService(viewOrTableName);
            StringBuilder stringBuilder = new StringBuilder();
            var filter = JsonConvert.DeserializeObject<JArray>(FunctionHelper.Base64Decode(pagingRequest.Filter));

            if (filter != null && filter.Type == JTokenType.Array)
            {
                BuildFilterClause(ref stringBuilder, filter);
            }
            else
            {
                stringBuilder.Append(" 1 = 1 ");
            }

            int totalRecord = await _baseRepository.CountTotalRecordByClause(stringBuilder.ToString(), viewOrTableName);



            return totalRecord;
        }

        /// <summary>
        /// Lấy bản ghi theo Id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi duy nhất</returns>
        public async Task<TEntity> GetEntityById(int entityId)
        {
            var entity = await _baseRepository.GetEntityById(entityId);
            return entity;
        }

        /// <summary>
        /// Thêm một thực thể
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public virtual async Task<ServiceResult> Insert(TEntity entity)
        {
            int dummy = 0;
            entity = await CustomValueWhenInsert(entity);
            entity.EntityState = EntityState.Add;

            //1. Validate tất cả các trường nếu được gắn thẻ
            var isValid = await Validate(entity, dummy);

            //2. Sử lí lỗi tương ứng
            if (isValid)
            {
                int rowAffects = await _baseRepository.Insert(entity);
                if (rowAffects == 0)
                {
                    _serviceResult.Code = Enums.Enums.Fail;
                    _serviceResult.Messasge = Properties.Resources.Msg_Failed;
                }
                else { _serviceResult.Data = rowAffects; }
            }

            AfterInsert();

            //3. Trả về kế quả
            return _serviceResult;
        }

        /// <summary>
        /// Lưu 1 phần
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdatePatch(int id, object model)
        {
            if(model == null)
            {
                throw new ArgumentNullException("Update patch Model is null."); 
            }

            var oldModel = await GetEntityById(id);
            if (oldModel == null)
            {
                _serviceResult.onError(null, $"Bản ghi với key={id} không tồn tại.");
                _serviceResult.Code = Enums.Enums.Fail;  
                return _serviceResult;
            }

            var modelDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(model?.ToString() ?? "{}");
            var oldModelDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(oldModel));
            oldModelDict = oldModelDict.Merge(modelDict);
            var entity = JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(oldModelDict));
            if(entity == null)
            {
                _serviceResult.onError(null, $"Bản ghi với key={id} không tồn tại.");
                return _serviceResult;
            }
            entity.ModifiedDate = DateTime.UtcNow;
            _serviceResult = await Update(id, entity);
            if (_serviceResult.Data != null && int.TryParse(_serviceResult.Data.ToString(), out int _))
            {
                _serviceResult.Data = id;
            }

            return _serviceResult;
        }

        /// <summary>
        /// Cập nhập thông tin bản ghi 
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<ServiceResult> Update(int entityId, TEntity entity)
        {

            var model = await GetEntityById(entityId);
            if (model == null)
            {
                _serviceResult.onError(_data: null, _message: null, _code: Enums.Enums.NotFound);
                return _serviceResult;
            }

            entity = CustomValueWhenUpdate(entity);
            entity.EntityState = EntityState.Update;

            var isValid = await Validate(entity, entityId);
            if (isValid)
            {
                int rowAffects = await _baseRepository.Update(entityId, entity);
                _serviceResult.Data = rowAffects;
                if (rowAffects > 0)
                {
                    _serviceResult.Code = Enums.Enums.Valid;
                    _serviceResult.Messasge = Properties.Resources.Msg_Success;
                }
                else
                {
                    _serviceResult.Code = Enums.Enums.InValid;
                    _serviceResult.Messasge = Properties.Resources.Msg_Failed;
                }
            }
            else
            {
                _serviceResult.Code = Enums.Enums.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
            }

            AfterUpdate();

            //3. Trả về kế quả
            return _serviceResult;
        }

        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<ServiceResult> Delete(int entityId)
        {
            // -- nếu không có thì trả về xóa thành công
            var entity = await _baseRepository.GetEntityById(entityId);
            if(entity == null)
            {
                _serviceResult.Data = 1;
                _serviceResult.Code = Enums.Enums.Success;
                _serviceResult.Messasge = Properties.Resources.Msg_Success;
                return _serviceResult;
            }

            int rowAffects = _baseRepository.Delete(entityId);
            _serviceResult.Data = rowAffects;

            if (rowAffects > 0)
            {
                _serviceResult.Code = Enums.Enums.Success;
                _serviceResult.Messasge = Properties.Resources.Msg_Success;
            }
            else
            {
                _serviceResult.Code = Enums.Enums.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_Failed;
            }

            AfterDelete();

            return _serviceResult;
        }

        /// <summary>
        /// Validate từng màn hình
        /// </summary>
        /// <param name="entity">Thực thể</param>
        protected virtual bool ValidateCustom(TEntity entity)
        {
            return true;
        }

        /// <summary>
        /// Custom lại value khi update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual TEntity CustomValueWhenUpdate(TEntity entity)
        {
            return entity;
        }

        /// <summary>
        /// Custom lại value khi insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> CustomValueWhenInsert(TEntity entity)
        {
            await Task.CompletedTask;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            return entity;
        }
        #endregion

        #region PRIVATE
        /// <summary>
        /// Validate tất cả
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        private async Task<bool> Validate(TEntity entity, int id)
        {
            var isValid = true;

            //1. Đọc các property
            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                //1.1 Kiểm tra xem  có attribute cần phải validate không
                if (isValid && property.IsDefined(typeof(IRequired), false))
                {
                    //1.1.1 Check bắt buộc nhập
                    isValid = ValidateRequired(entity, property);
                }
            }

            //2. Validate tùy chỉnh từng màn hình
            if (isValid)
            {
                isValid = ValidateCustom(entity);
            }

            //3. Validate trùng tên
            if (isValid)
            {
                isValid = await ValidateDulicate(entity, id);
            }

            return isValid;
        }
        /// <summary>
        /// Validate bắt buộc nhập
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        private bool ValidateRequired(TEntity entity, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Giấ trị
            var propertyValue = propertyInfo.GetValue(entity);

            //3. Tên hiển thị
            var propertyDisplayName = _modelType.GetColumnDisplayName(propertyName);

            if (string.IsNullOrEmpty(propertyValue?.ToString() ?? ""))
            {
                isValid = false;

                _serviceResult.Code = Enums.Enums.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_Required, propertyDisplayName);
            }

            return isValid;
        }

        /// <summary>
        /// Validate trùng
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        private async Task<bool> ValidateDulicate(TEntity entity, int id)
        {
            bool isValid = true;

            var uniqueColumns = _modelType.GetUniqueColumns().Split(";").ToList();
            var columns = _modelType.GetColumNames().ToList();
            if (columns.Intersect(uniqueColumns).ToList().Count == uniqueColumns.Count)
            {
                var cols = uniqueColumns.Select(f => $"{f} = @v_{f}");
                var query = new StringBuilder($"SELECT Count({_modelType.GetKeyName()}) FROM {_tableName} WHERE {string.Join(" AND ", cols)}");

                if (entity.EntityState == EntityState.Update)
                {
                    query.Append($" AND {_modelType.GetKeyName()} <> '{id}'");
                }

                if (_modelType.GetHasDeletedColumn())
                {
                    query.Append($" AND {nameof(entity.IsDeleted)} = FALSE ");
                }

                query.Append(";");

                var pars = uniqueColumns.ToDictionary(k => $"@v_{k}", v => _modelType.GetValueByFieldName(entity, v));
                var res = await _baseRepository.ExecuteScalaUsingCommandTextAsync<int>(query.ToString(), pars);
                isValid = res == 0;
            }

            if (!isValid)
            {
                _serviceResult.IsSuccess = isValid;
                _serviceResult.Code = Enums.Enums.Duplicate;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_Duplicate, _modelType.GetUniqueColumns());
            }

            return isValid;
        }
        #endregion

        #region HELPS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="optr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string BuildSingleOperator(string column, string optr, object value)
        {
            StringBuilder stringBuilder = new StringBuilder(column);
            FormatValueSql(ref value);

            if (optr == Operator.EQUAL)
            {
                stringBuilder.Append($" {Operator.EQUAL} ");
                stringBuilder.Append(value);
            }
            else if (optr == Operator.CONTAINS)
            {
                stringBuilder.Append($" LIKE CONCAT('%',{value},'%')");
            }
            else if (optr == Operator.START_WIDTH)
            {
                stringBuilder.Append($" LIKE CONCAT({value},'%')");
            }
            else if (optr == Operator.END_WIDTH)
            {
                stringBuilder.Append($" LIKE CONCAT('%',{value})");
            }
            else if (optr == Operator.NOT_EQUAL)
            {
                stringBuilder.Append($" {Operator.NOT_EQUAL} {value}");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void FormatValueSql(ref object value)
        {
            if (value.GetType() == typeof(string))
            {
                value = $"'{SecureHelper.SafeSqlLiteralForStringValue(value.ToString())}'";
            }
            else if (value.GetType() == typeof(TimeSpan))
            {
                value = $"'{((TimeSpan)value).ToString("HH:mm:ss")}'";
            }
            else if (value.GetType() == typeof(DateTime))
            {
                value = $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else if (value.GetType() == typeof(int) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double))
            {
                value = $"{value}";
            }
            else if (value.GetType() == typeof(bool))
            {
                value = (bool)value ? "TRUE" : "FALSE";
            }
            else if (value.GetType() == typeof(Guid))
            {
                value = $"'{((Guid)value).ToString()}'";
            }
        }
        /// <summary>
        /// BuildFilterClause
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="plainData"></param>
        public void BuildFilterClause(ref StringBuilder stringBuilder, JToken plainData)
        {
            if (plainData.Type == JTokenType.Array)
            {
                stringBuilder.Append(" ( ");

                JArray arr = (JArray)plainData;
                if (arr.Count == 3 && !arr.Any(item => item.Type == JTokenType.Array))
                {
                    string name = arr[0].Value<string>();
                    string opt = arr[1].Value<string>();
                    object value = HttpUtility.UrlDecode(arr[2].Value<string>());
                    stringBuilder.Append(BuildSingleOperator(name, opt, value));
                }
                else
                {
                    foreach (var jtoken in arr)
                    {
                        BuildFilterClause(ref stringBuilder, jtoken);
                    }
                }
                stringBuilder.Append(" ) ");
            }
            else if (plainData.Type == JTokenType.String)
            {
                if (string.Compare(plainData.Value<string>(), "and", true) == 0)
                {
                    stringBuilder.Append(" AND ");
                }
                else if (string.Compare(plainData.Value<string>(), "or", true) == 0)
                {
                    stringBuilder.Append(" OR ");
                }
            }
        }
        #endregion

        #region Overrides
        protected virtual void AfterInsert()
        {
        }
        protected virtual void AfterUpdate()
        {
        }
        protected virtual void AfterDelete()
        {
        }
        #endregion
    }
}
