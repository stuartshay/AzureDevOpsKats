using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Web.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    /// <summary>
    /// Cats Controller
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/cats")]
   // [EnableCors("AllowAll")]
    [ApiVersion("1.0")]
    public class CatsController : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly IFileService _fileService;

        private readonly ILogger<CatsController> _logger;

        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatsController"/> class.
        /// </summary>
        /// <param name="catService"></param>
        /// <param name="fileService"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="settings"></param>
        public CatsController(ICatService catService, IFileService fileService, ILogger<CatsController> logger, IWebHostEnvironment env, IOptions<ApplicationOptions> settings)
        {
            _catService = catService ?? throw new ArgumentNullException(nameof(catService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationSettings = settings.Value;
            _env = env;

            _logger.LogInformation("Init CatsController-1: {Now}", DateTime.Now);
        }

        private ApplicationOptions ApplicationSettings { get; set; }

        /// <summary>
        /// Get List of Cats
        /// </summary>
        /// <returns>An ActionResult of type Cat List</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<CatModel>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CatModel>>> Get()
        {
            using (_logger.BeginScope(new Dictionary<string, object> { { "MyKey", "MyValue" } }))
            {
                _logger.LogWarning("Get All Cats");
                _logger.LogError("An example of an Error level message");
            }

            var results = await _catService.GetCats();
            return Ok(results);
        }

        /// <summary>
        /// Get Cat
        /// </summary>
        /// <param name="id">Cat Id</param>
        /// <returns>An ActionResult of type Cat</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpGet("{id}", Name = "GetById")]
        [Produces("application/json", Type = typeof(CatModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatModel>> GetById(int id)
        {
            var result = await _catService.GetCat(id);
            if (result == null)
                return NotFound();
           
            _logger.LogInformation("Get:{id}", id);

            return Ok(result);
        }

        /// <summary>
        /// Delete Cat
        /// </summary>
        /// <param name="id">Cat Id</param>
        /// <returns>No Content Result</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _catService.GetCat(id);
            if (result == null)
                return NotFound();

            await _catService.DeleteCat(id);
            _fileService.DeleteFile(result.Photo);

            return NoContent();
        }

        /// <summary>
        ///  Create Cat
        /// </summary>
        /// <param name="value">Cat Create Model</param>
        /// <returns>Cat Model</returns>
        [MapToApiVersion("1.0")]
        [HttpPost]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> Post([FromBody] CatCreateModel value)
        {
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            $"Bytes Exist:{value.Bytes != null}".ConsoleRed();

            string fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(ApplicationSettings.FileStorage.PhysicalFilePath, fileName);
            _logger.LogInformation("Save Image: {FilePath}", filePath);

            var catModel = new CatModel
            {
                Name = value.Name,
                Description = value.Description,
                Photo = fileName,
            };

            _fileService.SaveFile(filePath, value.Bytes);
            var result = await _catService.CreateCat(catModel);
            catModel.Id = result;

            return CreatedAtRoute("GetById", new { Id = result }, catModel);
        }

        /// <summary>
        /// Update Cat Properties
        /// </summary>
        /// <param name="id">Cat Id</param>
        /// <param name="value">Cat Update Model</param>
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
        public async Task<ActionResult> Put(int id, [FromBody] CatUpdateModel value)
        {
            var result = await _catService.GetCat(id);
            if (result == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            await _catService.EditCat(id, value);

            _logger.LogInformation("Update:{id}|{@value}", id, value);

            return Ok(value);
        }
    }
}
