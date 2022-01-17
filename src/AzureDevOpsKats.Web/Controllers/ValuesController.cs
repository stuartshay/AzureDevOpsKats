using AzureDevOpsKats.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v{version:apiVersion}/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly SmtpConfiguration _smtpConfiguration;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public ValuesController(IOptions<SmtpConfiguration> settings)
        {
            _smtpConfiguration = settings.Value;
        }

        /// <summary>
        ///  AWS Parameter Store Configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SmtpConfiguration Get()
        {
            return _smtpConfiguration;
        }

    }
}
