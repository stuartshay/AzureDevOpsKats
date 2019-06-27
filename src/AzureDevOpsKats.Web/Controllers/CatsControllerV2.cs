using System;
using System.IO;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    using System.Threading.Tasks;

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

        private readonly ILogger<CatsController> _logger;

        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsControllerV2"/> class.
        /// </summary>
        /// <param name="catService"></param>
        /// <param name="logger"></param>
        public CatsControllerV2(ICatService catService, IFileService fileService, ILogger<CatsController> logger, IHostingEnvironment env, IOptions<ApplicationOptions> settings)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationSettings = settings.Value;
            _env = env;
        }

        private ApplicationOptions ApplicationSettings { get; set; }

        /// <summary>
        /// Get Cats Paging
        /// </summary>
        /// <param name="limit">results count</param>
        /// <param name="page">page number</param>
        /// <returns></returns>
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

            var results = _catService.GetCats(limit, page*limit);

            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
            HttpContext.Response.Headers.Add("X-InlineCount", total.ToString());

            return Ok(results);
        }

        /// <summary>
        ///  Total Cat Photos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("results/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTotal()
        {
            var results = _catService.GetCount();
            return Ok(results);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="form"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [MapToApiVersion("2.0")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAsync(IFormCollection form)
        {
            _logger.LogInformation("inputed form data:", form);

            string fileName = $"{Guid.NewGuid()}.jpg";

            string imageDirectory = ApplicationSettings.FileStorage.FilePath;
            var filePath = Path.Combine(_env.ContentRootPath, imageDirectory, fileName);

            var catModel = new CatModel
            {
                Name = form["name"],
                Description = form["description"],
                Photo = fileName,
            };

            if (form.Files.Count > 0)
            {
                IFormFile file = form.Files[0];

                if (file == null || file.Length == 0)
                    throw new Exception("File is empty!");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var result = _catService.CreateCat(catModel);
            catModel.Id = result;

            return CreatedAtRoute("GetById", new { Id = result }, catModel);
        }
    }
}
