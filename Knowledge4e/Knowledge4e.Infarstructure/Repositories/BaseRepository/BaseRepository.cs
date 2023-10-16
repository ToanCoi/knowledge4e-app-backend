using Dapper;
using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Extensions;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Infarstructure.Repositories.BaseRepository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : BaseEntity
    {
        #region Declare
        IConfiguration _configuration;
        protected IDbConnection _dbConnection = null;
        string _connectionString = string.Empty;
        protected string _tableName;
        protected string _databaseName;
        public Type _modelType = null;
        #endregion

        #region Constructer
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AppConnectionString");
            _databaseName = _configuration["Database"];
            _dbConnection = new MySqlConnection(_connectionString);
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName().ToLowerInvariant();
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Lấy tất cả
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetEntities()
        {
            return await GetEntitiesUsingCommandTextAsync();
        }

        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<TEntity> GetEntityById(int entityId)
        {
            var entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());
            return entity;
        }

        /// <summary>
        /// Xóa theo mã
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public int Delete(int entityId)
        {
            var rowAffects = 0;
            OpenConnection();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var keyName = _modelType.GetKeyName();

                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@v_{keyName}", entityId);

                    var query = new StringBuilder($"DELETE FROM {_tableName} WHERE {keyName}=@v_{keyName}");
                    if (_modelType.GetHasDeletedColumn())
                    {
                        query = new StringBuilder($"UPDATE {_tableName} SET IsDeleted=TRUE WHERE {keyName}=@v_{keyName}");
                    }

                    rowAffects = _dbConnection.Execute(query.ToString(), param: dynamicParams, transaction: transaction, commandType: CommandType.Text);

                    transaction.Commit();
                }
                catch(Exception ex) 
                { 
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (_dbConnection.State == ConnectionState.Open)
                    {
                        Dispose();
                    }
                }
            }

            //3. Trả về số bản ghi bị ảnh hưởng
            return rowAffects;
        }

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Insert(TEntity entity)
        {
            var rowAffects = 0;

            OpenConnection();
            var columnsInDatabase = await GetTableColumnsInDatabase();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var query = SqlExtensions.GenerateInsertQuery(_modelType, columnsInDatabase.ToList());

                    if (string.IsNullOrEmpty(query)) return rowAffects;

                    var parameters = _modelType.MappingDbType(entity);

                    rowAffects = await _dbConnection.ExecuteAsync(query, param: parameters, transaction: transaction, commandType: CommandType.Text);
                    transaction.Commit();
                    await AfterInsertCommit(transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (_dbConnection.State == ConnectionState.Open)
                    {
                        Dispose();
                    }
                }
            }

            //3.Trả về số bản ghi thêm mới
            return rowAffects;
        }

        /// <summary>
        /// Cập nhập bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Update(int entityId, TEntity entity)
        {
            var rowAffects = 0;
            OpenConnection();

            var columnsInDatabase = await GetTableColumnsInDatabase();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var query = SqlExtensions.GenerateUpdateQuery(_modelType, columnsInDatabase.ToList());

                    if (string.IsNullOrEmpty(query)) return rowAffects;

                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    var parameters = _modelType.MappingDbType(entity);

                    //3. Kết nối tới CSDL:
                    rowAffects = await _dbConnection.ExecuteAsync(query.ToString(), param: parameters, transaction: transaction, commandType: CommandType.Text);
                    transaction.Commit();
                    await AfterUpdateCommit(transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (_dbConnection.State == ConnectionState.Open)
                    {
                        Dispose();
                    }
                }
            }
            //4. Trả về dữ liệu
            return rowAffects;
        }

        /// <summary>
        /// Lấy thưc thể theo thuộc tính
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="property">Thuộc tính trong thực thể</param>
        /// <returns>Thực thể</returns>
        public TEntity GetEntityByProperty(TEntity entity, PropertyInfo property)
        {
            //1. Thông tin của trường hiện tại
            var propertyName = property.Name;
            var propertyValue = property.GetValue(entity);

            //2. Thông tin khóa
            var keyName = _modelType.GetKeyName();
            var keyValue = _modelType.GetKeyValue(entity);

            string query = string.Empty;

            //3. Kiểm tra kiểu form
            if (entity.EntityState == EntityState.Add)
                query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}' AND IsDeleted = FALSE";
            else if (entity.EntityState == EntityState.Update)
                query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}' AND {keyName} <> '{keyValue}' AND IsDeleted = FALSE";
            else
                return null;

            var entityReturn = _dbConnection.Query<TEntity>(query, commandType: CommandType.Text).FirstOrDefault();
            return entityReturn;
        }

        /// <summary>
        /// Lấy thưc thể theo thuộc tính
        /// </summary>
        /// <param name="propertyName">Thuộc tính</param>
        /// <param name="propertyValue">Giá trị của thuộc tính</param>
        /// <returns>Thực thể</returns>
        public IEnumerable<TEntity> GetEntitiesByProperty(string propertyName, object propertyValue)
        {
            string query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}'";
            var entityReturn = _dbConnection.Query<TEntity>(query, commandType: CommandType.Text);
            return entityReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetEntitiesFilter(string whereClause, string columnNames = "*", string viewName = "")
        {
            string resource = _tableName;
            if (!string.IsNullOrEmpty(viewName))
            {
                resource = viewName;
            }

            if (columnNames == null) columnNames = "*";
            string query = $"SELECT {columnNames} FROM {resource} WHERE {whereClause}";
            var entityReturn = await _dbConnection.QueryAsync<TEntity>(query, commandType: CommandType.Text);
            return entityReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public async Task<int> CountTotalRecordByClause(string whereClause, string viewName = "")
        {
            string resource = _tableName;
            if (!string.IsNullOrEmpty(viewName))
            {
                resource = viewName;
            }
            string queryTotal = $"SELECT COUNT(*) FROM {resource} WHERE {whereClause}";
            var totalRecord = await _dbConnection.QuerySingleAsync<int>(queryTotal, commandType: CommandType.Text);
            return totalRecord;
        }

        /// <summary>
        /// Đóng kết nối
        /// </summary>
        public void Dispose()
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
            }
        }

        public async Task<IEnumerable<TEntity>> QueryUsingCommandTextAsync(string commandText, object pars = null) => (await _dbConnection.QueryAsync<TEntity>(commandText, param: pars, commandType: CommandType.Text)).ToList();

        public async Task<T> ExecuteScalaUsingCommandTextAsync<T>(string commandText, object pars = null) => await _dbConnection.ExecuteScalarAsync<T>(commandText, param: pars, commandType: CommandType.Text);

        /// <summary>
        /// Lấy column trong table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetTableColumnsInDatabase(string table = "")
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = _tableName;
            }
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException($"Không có table name với model: {_modelType.Name}");
            }

            //TODO: cache lại data
            var query = new StringBuilder($"SELECT COLUMN_NAME FROM information_schema.columns WHERE table_name='{table}' and TABLE_SCHEMA='{_databaseName}'");
            var columns = await _dbConnection.QueryAsync<string>(query.ToString(), commandType: CommandType.Text);

            return columns;
        }

        #endregion

        #region PRIVATE
        /// <summary>
        /// Lấy tất cả theo command text
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            if (_modelType.GetHasDeletedColumn())
            {
                whereCount++;
                query.Append($" where IsDeleted = FALSE");
            }

            var entities = await _dbConnection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities.ToList();
        }

        /// <summary>
        /// Lấy bản ghi theo id dùng command text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<TEntity> GetEntitieByIdUsingCommandTextAsync(string id)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            Func<StringBuilder, bool> AppendWhere = (query) => { if (whereCount == 0) query.Append(" where "); return true; };

            var primaryKey = _modelType.GetKeyName();

            if (primaryKey != null)
            {
                AppendWhere(query);
                query.Append($"{primaryKey} = '{id}'");
                whereCount++;
            }

            if (_modelType.GetHasDeletedColumn())
            {
                AppendWhere(query);
                query.Append($" AND IsDeleted = FALSE");
                whereCount++;
            }

            var entities = await _dbConnection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<TEntity>> QueryUsingProcedureAsync(string procedure, object pars = null)
        {
            return (await _dbConnection.QueryAsync<TEntity>(procedure, param: pars, commandType: CommandType.StoredProcedure)).ToList();
        }

        /// <summary>
        /// Mở connection
        /// </summary>
        /// <returns></returns>
        private void OpenConnection()
        {
            if (_dbConnection == null || _dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Insert đã được commit
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async virtual Task AfterInsertCommit(IDbTransaction transaction)
        {
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Update đã được commit
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async virtual Task AfterUpdateCommit(IDbTransaction transaction)
        {
            await Task.CompletedTask;
        }
        #endregion
    }
}