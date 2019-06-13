using System;
using System.Collections.Generic;
using System.IO;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Web.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/cats")]
    [EnableCors("AllowAll")]
    [ApiVersion("1.0")]
    public class CatsController : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly IFileService _fileService;

        private readonly ILogger<CatsController> _logger;

        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsController"/> class.
        /// </summary>
        /// <param name="catService"></param>
        /// <param name="fileService"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="settings"></param>
        public CatsController(ICatService catService, IFileService fileService, ILogger<CatsController> logger, IHostingEnvironment env, IOptions<ApplicationOptions> settings)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationSettings = settings.Value;
            _env = env;
        }

        private ApplicationOptions ApplicationSettings { get; set; }

        /// <summary>
        /// Get List of Cats
        /// </summary>
        /// <returns>An ActionResult of type Cat List</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<CatModel>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            using (_logger.BeginScope(new Dictionary<string, object> { { "MyKey", "MyValue" } }))
            {
                _logger.LogWarning("Get All Cats");
                _logger.LogError("An example of an Error level message");
            }

            var results = _catService.GetCats();
            return Ok(results);
        }

        /// <summary>
        /// Get Cat
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An ActionResult of type Cat</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpGet("{id}", Name = "GetById")]
        [Produces("application/json", Type = typeof(CatModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var result = _catService.GetCat(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Delete Cat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var result = _catService.GetCat(id);
            if (result == null)
                return NotFound();

            _catService.DeleteCat(id);
            _fileService.DeleteFile(result.Photo);

            return NoContent();
        }

        /// <summary>
        ///  Create Cat
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CatModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Post([FromBody] CatCreateModel value)
        {
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            $"Bytes Exist:{value.Bytes != null}".ConsoleRed();

            string fileName = $"{Guid.NewGuid()}.jpg";
            string imageDirectory = ApplicationSettings.FileStorage.FilePath;
            var filePath = Path.Combine(_env.ContentRootPath, imageDirectory, fileName);

            var catModel = new CatModel
            {
                Name = value.Name,
                Description = value.Description,
                Photo = fileName,
            };

            _fileService.SaveFile(filePath, value.Bytes);
            var result = _catService.CreateCat(catModel);
            catModel.Id = result;

            return CreatedAtRoute("GetById", new { Id = result }, catModel);
        }

        /// <summary>
        /// Update Cat Properties
        /// </summary>
        /// <param name="id">Cat Id</param>
        /// <param name="value"></param>
        /// <response code="200">Returns the updated cat</response>
        /// <response code="422">Validation error</response>
        /// <returns>An ActionResult of type Cat</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]
        public IActionResult Put(int id, [FromBody] CatUpdateModel value)
        {
            var result = _catService.GetCat(id);
            if (result == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _catService.EditCat(id, value);

            return Ok(value);
        }
    }
}
