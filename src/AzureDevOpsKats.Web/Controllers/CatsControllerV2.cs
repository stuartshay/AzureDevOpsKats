using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    /// CatsController V2
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/cats")]
    // [EnableCors("AllowAll")]
    [ApiVersion("2.0")]
    public class CatsControllerV2 : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly IFileService _fileService;

        private readonly ILogger<CatsControllerV2> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsControllerV2"/> class.
        /// </summary>
        /// <param name="catService">Cat Service</param>
        /// <param name="fileService">File Service</param>
        /// <param name="logger">Logger</param>
        /// <param name="settings"></param>
        public CatsControllerV2(ICatService catService, IFileService fileService, ILogger<CatsControllerV2> logger, IOptions<ApplicationOptions> settings)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            ApplicationSettings = settings.Value;

            _logger.LogInformation("Init CatsControllerV2-1: {Now}", DateTime.Now);
        }

        private ApplicationOptions ApplicationSettings { get; set; }

        /// <summary>
        /// Get Cats Paging
        /// </summary>
        /// <param name="limit">Results count</param>
        /// <param name="page">Page Number</param>
        /// <returns>List of Cats</returns>
        [HttpGet]
        [Route("{limit:int}/{page:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<CatModel>>> Get(int limit, int page)
        {
            var total = await _catService.GetCount();
            if (total <= 0)
            {
                return NotFound();
            }

            var results = await _catService.GetCats(limit, page * limit);

            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
            HttpContext.Response.Headers.Add("X-InlineCount", total.ToString(CultureInfo.InvariantCulture));

            _logger.LogInformation("Web Get All Cats|Limit:{limit}|Page:{page}", limit, page);

            return Ok(results);
        }

        /// <summary>
        ///  Total Cat Photos
        /// </summary>
        /// <returns>Total Cats</returns>
        [HttpGet]
        [Route("results/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<long>> GetTotal()
        {
            var results = await _catService.GetCount();

            _logger.LogInformation("Get Total:{results}:", results);

            return Ok(results);
        }

        /// <summary>
        ///  Create Cat
        /// </summary>
        /// <param name="value">Cat Create Model</param>
        /// <returns>Cat Model</returns>
        [MapToApiVersion("2.0")]
        [HttpPost]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> Post([FromForm] CatCreateViewModel value)
        {
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (value.File != null)
            {
                IFormFile file = value.File;

                List<string> imgErrors = new List<string>();
                var supportedTypes = new[] { "png", "jpg" };
                var fileExt = Path.GetExtension(file.FileName).Substring(1);
                if (file.Length == 0)
                {
                    imgErrors.Add("File is empty!");
                }

                if (Array.IndexOf(supportedTypes, fileExt) < 0)
                {
                    imgErrors.Add("File Extension Is InValid - Only Upload image File");
                }

                if (imgErrors.Count > 0)
                {
                    return BadRequest(new { Image = imgErrors });
                }

                string fileName = $"{Guid.NewGuid()}.{fileExt}";
                var filePath = Path.Combine(ApplicationSettings.FileStorage.PhysicalFilePath, fileName);

                _logger.LogDebug($"Save Image:{filePath}");
                _fileService.SaveFile(filePath, FormFileBytes(file));

                var catModel = new CatModel
                {
                    Name = value.Name,
                    Description = value.Description,
                    Photo = fileName,
                };

                var result = await _catService.CreateCat(catModel);
                catModel.Id = result;

                return CreatedAtRoute("GetById", new { Id = result }, catModel);
            }
            else
            {
                List<string> imgErrors = new List<string> { "File is empty!" };
                return BadRequest(new { errors = new { Image = imgErrors } });
            }
        }

        private static byte[] FormFileBytes(IFormFile file)
        {
            byte[] bytes = null;

            if (file.Length <= 0)
            {
                return bytes;
            }

            using var fileStream = file.OpenReadStream();
            using var ms = new MemoryStream();

            fileStream.CopyTo(ms);
            bytes = ms.ToArray();

            return bytes;
        }
    }
}
