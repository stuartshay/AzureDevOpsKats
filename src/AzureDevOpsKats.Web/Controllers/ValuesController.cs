using AzureDevOpsKats.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        ///  Determine Directory Exists.
        /// </summary>
        /// <returns>Total Cats</returns>
        [HttpGet]
        [Route("directory/exist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> DirectoryExists()
        {
            string root = @"/images";
            var results = false;
            if (Directory.Exists(root))
            {
                results = true;
            }

            return Ok(results);
        }

    }
}
