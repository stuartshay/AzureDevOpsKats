using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Web.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    /// CatsController V2
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/cats")]
    [EnableCors("AllowAll")]
    [ApiVersion("2.0")]
    public class CatsControllerV2 : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly ILogger<CatsController> _logger;

        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsControllerV2"/> class.
        /// </summary>
        /// <param name="catService"></param>
        /// <param name="fileService"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="settings"></param>
        public CatsControllerV2(ICatService catService, IFileService fileService, ILogger<CatsController> logger, IHostingEnvironment env, IOptions<ApplicationOptions> settings)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env;
            ApplicationSettings = settings.Value;
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
        public IActionResult Get(int limit, int page)
        {
            _logger.LogWarning("Get All Cats");

            var total = _catService.GetCount();
            if (total == 0)
                return NotFound();

            var results = _catService.GetCats(limit, page * limit);

            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
            HttpContext.Response.Headers.Add("X-InlineCount", total.ToString(CultureInfo.InvariantCulture));

            return Ok(results);
        }

        /// <summary>
        ///  Total Cat Photos
        /// </summary>
        /// <returns>Total Cats</returns>
        [HttpGet]
        [Route("results/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTotal()
        {
            var results = _catService.GetCount();
            return Ok(results);
        }

        /// <summary>
        /// Create Cat
        /// </summary>
        /// <param name="form">IFormCollection Interface</param>
        /// <returns>Cat Model</returns>
        [MapToApiVersion("2.0")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAsync(IFormCollection form)
        {
            string fileName = $"{Guid.NewGuid()}.jpg";
            string imageDirectory = ApplicationSettings.FileStorage.FilePath;
            var filePath = Path.Combine(_env.ContentRootPath, imageDirectory, fileName);

            var catModel = new CatModel
            {
                Name = form["name"],
                Description = form["description"],
                Photo = fileName,
            };

            TryValidateModel(catModel);

            if (!ModelState.IsValid)
            {
                var returnValue = new UnprocessableEntityObjectResult(ModelState);
                return returnValue;
            }

            if (form.Files.Count > 0)
            {
                IFormFile file = form.Files[0];

                List<string> imgErrors = new List<string>();
                var supportedTypes = new[] { "png", "jpg", "bmp", "ico" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (file == null || file.Length == 0)
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

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            else
            {
                List<string> imgErrors = new List<string>();
                imgErrors.Add("File is empty!");
                return BadRequest(new { Image = imgErrors });
            }

            var result = _catService.CreateCat(catModel);
            catModel.Id = result;

            catModel.Id = result;
            return CreatedAtRoute("GetById", new { Id = result }, catModel);
        }
    }
}
