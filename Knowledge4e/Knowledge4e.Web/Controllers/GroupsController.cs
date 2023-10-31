using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Services.BaseService;
using Knowledge4e.Entities.Entities.Group;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Knowledge4e.Web.Controllers
{
    /// <summary>
    /// Controller group
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GroupsController : BaseEntityController<KGroup>
    {
        #region Declare
        ILogger<KGroup> _logger;
        IGroupService _groupService;
        #endregion

        #region Constructer
        public GroupsController(ILogger<KGroup> logger, IGroupService accountService) : base(accountService, logger)
        {
            _logger = logger;
            _groupService = accountService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Test Authorization
        /// </summary>
        [EnableCors("AllowCROSPolicy")]
        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok(new string[] { "value1", "value2", "value3", "value4", "value5" });
        }
        /// <summary>
        #endregion
    }
}
