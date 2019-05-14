using System;
using System.Collections.Generic;
using AzureDevOpsKats.Service.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/cats")]
    [EnableCors("AllowAll")]
    [ApiVersion("2.0")]
    public class CatsControllerV2 : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly IFileService _fileService;

        private readonly ILogger<CatsControllerV2> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsController"/> class.
        /// </summary>
        /// <param name="catService"></param>
        public CatsControllerV2(ICatService catService)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
        }

        /// <summary>
        /// Get Cats Paging
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
