using Microsoft.AspNetCore.Http;
using Knowledge4e.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Knowledge4e.ApplicationCore.Interfaces
{
    public interface IBaseService<TEntity>
    {

        /// <summary>
        /// Lấy danh sách bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        Task<IEnumerable<TEntity>> GetEntities();

        /// <summary>
        /// Lấy danh sách bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        Task<ServiceResult> GetEntitiesFilter(PagingRequest pagingRequest, string viewOrTableName = "");
        Task<ServiceResult> GetEntitiesFilter<T>(PagingRequest pagingRequest, string viewOrTableName = "");

        /// <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</returns>
        Task<TEntity> GetEntityById(Guid entityId);

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi</returns>
        Task<ServiceResult> Insert(TEntity entity);

        /// <summary>
        /// Cập nhập thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<ServiceResult> Update(Guid entityId, TEntity entity);



        /// <summary>
        /// Cập nhập một phần thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public Task<ServiceResult> UpdatePatch(Guid id, object model);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Số dòng bị xóa</returns>
        ServiceResult Delete(Guid entityId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="viewOrTableName"></param>
        /// <returns></returns>
        Task<int> CountTotalRecordByClause(PagingRequest pagingRequest, string viewOrTableName = "");

        /// <summary>
        /// Đọc file excel
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //Task<ServiceResult> readExcelFile(IFormFile formFile, CancellationToken cancellationToken);

        /// <summary>
        /// Thêm nhiều bản ghi
        /// </summary>
        /// <param name="ieEntities"></param>
        /// <returns></returns>\
        //ServiceResult MultiInsert(IEnumerable<TEntity> ieEntities);
    }
}
