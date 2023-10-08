using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Knowledge4e.ApplicationCore.Entities;
using Knowledge4e.ApplicationCore.Interfaces;
using Knowledge4e.Entities;

namespace Knowledge4e.Web.Controllers
{
    /// <summary>
    /// Controller base
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class BaseEntityController<TEntity> : ControllerBase
    {
        #region Declare
        IBaseService<TEntity> _baseService;
        private readonly ILogger<TEntity> _logger;
        #endregion

        public BaseEntityController(IBaseService<TEntity> baseService, ILogger<TEntity> logger)
        {
            _baseService = baseService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách thực thể
        /// </summary>
        /// <returns>Danh sách thực thể</returns>
        [EnableCors("AllowCROSPolicy")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var entities = await _baseService.GetEntities();

            return Ok(entities);
        }

        [HttpPost]
        [Route("Filter")]
        [EnableCors("AllowCROSPolicy")]
        public virtual async Task<IActionResult> GetFilter(PagingRequest pagingRequest)
        {
            var serviceResult = new ServiceResult();
            try
            {
                _logger.LogInformation($"Filter {typeof(TEntity).Name} info : " + JsonConvert.SerializeObject(pagingRequest));
                var entity = await _baseService.GetEntitiesFilter(pagingRequest);

                if (entity == null)
                    return NotFound();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi GetFilter: " + ex.Message);
                serviceResult.Data = null;
                serviceResult.Messasge = ex.Message;
                serviceResult.Code = Enums.Fail;
            }

            if (serviceResult.Code == Enums.Fail) { return BadRequest(serviceResult); }

            return Ok(serviceResult);
        }

        /// <summary>
        /// Lấy thực thể theo id
        /// </summary>
        /// <param name="id">id của thực thể</param>
        /// <returns>Một thực thể tìm được theo id</returns>
        [EnableCors("AllowCROSPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var entity = await _baseService.GetEntityById(Guid.Parse(id));

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        /// <summary>
        /// Thêm một thực thể mới
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Sô bản ghi bị ảnh hưởng</returns>
        [EnableCors("AllowCROSPolicy")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] TEntity entity)
        {
            var serviceResult = new ServiceResult();
            try
            {
                _logger.LogInformation($"Thêm bản ghi {typeof(TEntity).Name}: " + JsonConvert.SerializeObject(entity));
                serviceResult = await _baseService.Insert(entity);
                if (serviceResult.Code == Enums.InValid)
                    return BadRequest(serviceResult);
                else if (serviceResult.Code == Enums.Exception || serviceResult.Code == Enums.Fail)
                    return StatusCode(500, serviceResult);

                return StatusCode(201, serviceResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi Insert {typeof(TEntity).Name}: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [EnableCors("AllowCROSPolicy")]
        [HttpPut("PatchUpdate/{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(Guid id, [FromBody] object model)
        {
            try
            {
                var serviceResult = new ServiceResult();
                if (model != null)
                {
                    serviceResult = await _baseService.UpdatePatch(id, model);

                    if (serviceResult.Code == Enums.InValid)
                        return BadRequest(serviceResult);
                    else if (serviceResult.Code == Enums.Exception)
                        return StatusCode(500, serviceResult);

                    return Ok(serviceResult.Data);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Sửa một thực thể
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <param name="entity">thông tin của bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        [EnableCors("AllowCROSPolicy")]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] TEntity entity)
        {
            try
            {
                _logger.LogInformation($"Body put {typeof(TEntity).Name}:" + JsonConvert.SerializeObject(entity));
                //Sử lí kiểu id động ở đây
                var serviceResult = await _baseService.Update(Guid.Parse(id), entity);
                _logger.LogInformation($"ServiceResult Body put {typeof(TEntity).Name}:" + JsonConvert.SerializeObject(serviceResult));

                if (serviceResult.Code == Enums.InValid)
                    return StatusCode(StatusCodes.Status400BadRequest, serviceResult);
                else if (serviceResult.Code == Enums.Exception)
                    return StatusCode(StatusCodes.Status500InternalServerError, serviceResult);
                else if (serviceResult.Code == Enums.NotFound)
                    return StatusCode(StatusCodes.Status404NotFound, serviceResult);

                return Ok(serviceResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error put {typeof(TEntity).Name}:" + ex.Message);
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        [EnableCors("AllowCROSPolicy")]
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var serviceResult = _baseService.Delete(Guid.Parse(id));
            if (serviceResult.Code == Enums.Success)
                return Ok(serviceResult);
            else
                return NoContent();
        }
    }
}
