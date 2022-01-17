using AzureDevOpsKats.Common.Configuration;
using Microsoft.AspNetCore.Http;
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
        private readonly string _server;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public ValuesController(IOptions<SmtpConfiguration> settings)
        {
            _server = settings.Value.Server;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            var result = $"Server:{_server}";
            return result;
        }

    }
}
