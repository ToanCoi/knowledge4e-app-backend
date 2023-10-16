using Knowledge4e.Core.Entities.Account;
using Knowledge4e.Core.Services.BaseService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Knowledge4e.Web.Controllers
{
    /// <summary>
    /// Controller account
    /// </summary>
    [ApiController]
    public class AccountsController : BaseEntityController<Account>
    {
        #region Declare
        ILogger<Account> _logger;
        IAccountService _accountService;
        #endregion

        #region Constructer
        public AccountsController(ILogger<Account> logger, IAccountService accountService) : base(accountService, logger)
        {
            _logger = logger;
            _accountService = accountService;
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
