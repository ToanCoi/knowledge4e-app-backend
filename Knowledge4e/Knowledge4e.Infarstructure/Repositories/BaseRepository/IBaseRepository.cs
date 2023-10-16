using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Infarstructure.Repositories.BaseRepository
{
    public interface IBaseRepository<TEntity>
    {
        /// <summary>
        /// Lấy danh sách bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        Task<IEnumerable<TEntity>> GetEntities();

        /// <summary>
        /// Base filter
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetEntitiesFilter(string whereClause, string columnNames = "*", string viewName = "");

        /// <summary>
        /// Base filter
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        Task<int> CountTotalRecordByClause(string whereClause, string viewName = "");

        /// <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</return

        Task<TEntity> GetEntityById(int entityId);

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="enitity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi</returns>

        Task<int> Insert(TEntity enitity);

        /// <summary>
        /// Cập nhập thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> Update(int entityId, TEntity entity);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Số bản ghi bị xóa</returns>
        int Delete(int entityId);

        /// <summary>
        /// Lấy bản ghi theo thuộc tính
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="property">Thuộc tính trong bản ghi</param>
        /// <returns>Thực thể</returns>
        TEntity GetEntityByProperty(TEntity entity, PropertyInfo property);

        /// <summary>
        /// Lấy bản ghi theo thuộc tính
        /// </summary>
        /// <param name="propertyName">Tên thuộc tính</param>
        /// <param name="propertyValue">Giá trị của thuộc tính</param>
        /// <returns>Thực thể</returns>
        IEnumerable<TEntity> GetEntitiesByProperty(string propertyName, object propertyValue);

        /// <summary>
        /// Query lấy bản ghi dùng command text
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryUsingCommandTextAsync(string commandText, object pars = null);

        /// <summary>
        /// Query lấy số bản ghi dùng command text
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        Task<T> ExecuteScalaUsingCommandTextAsync<T>(string commandText, object pars = null);

        /// <summary>
        /// Query lấy bản ghi dùng procedure
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryUsingProcedureAsync(string procedure, object pars = null);

        /// <summary>
        /// Lấy column trong table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetTableColumnsInDatabase(string table = "");

        /// <summary>
        /// Insert nhưng chưa commit
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task AfterInsertCommit(IDbTransaction transaction);
    }
}
