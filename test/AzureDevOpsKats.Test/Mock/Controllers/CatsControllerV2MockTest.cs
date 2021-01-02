using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Test.Fixture;
using AzureDevOpsKats.Test.Helpers;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace AzureDevOpsKats.Test.Mock.Controllers
{
    public class CatsControllerV2MockTest : IClassFixture<CatConfigurationFixture>
    {
        private readonly ITestOutputHelper _output;

        private readonly ServiceProvider _serviceProvider;

        public CatsControllerV2MockTest(CatConfigurationFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Total_Cats_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCount()).ReturnsAsync(2);

            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetCatsControllerV2(httpResponse, mockCatService.Object);
            controller.ControllerContext = MockHelpers.GetHttpContext();

            // Act
            var sut = await controller.GetTotal();
            _output.WriteLine($"Total:{sut}");

            // Assert 
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut.Result);

            var objectResult = sut.Result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            Assert.IsType<long>(objectResult.Value);
            Assert.Equal(2L, objectResult.Value);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cats_Paging_List_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCount()).ReturnsAsync(2);
            mockCatService.Setup(b => b.GetCats(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<CatModel>()
                {
                    new CatModel{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new CatModel{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetCatsControllerV2(httpResponse, mockCatService.Object);
            controller.ControllerContext = MockHelpers.GetHttpContext();

            // Act
            var sut = controller.Get(2, 1);

            // Assert 
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            //var objectResult = sut.Result as OkObjectResult;
            //Assert.NotNull(objectResult);
            //Assert.True(objectResult.StatusCode == 200);

            //var cats = objectResult.Value as List<CatModel>;
            //Assert.NotNull(cats);
            //Assert.NotEmpty(cats);
            //Assert.True(cats.Count == 2);
            //Assert.Equal(1, cats[0].Id);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cats_Paging_List_NotFound()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCount()).ReturnsAsync(0);

            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.NotFound);
            var controller = GetCatsControllerV2(httpResponse, mockCatService.Object);

            var sut = await controller.Get(2, 1).ConfigureAwait(false);

            Assert.NotNull(sut);
            Assert.IsType<ActionResult<IEnumerable<CatModel>>>(sut);
            Assert.IsType<NotFoundResult>(sut.Result);
        }

        private CatsControllerV2 GetCatsControllerV2(
            HttpResponseMessage responseMessage, 
            ICatService catService = null,
            IFileService fileService = null,
            ILogger<CatsControllerV2> logger = null,
            IOptions<ApplicationOptions> settings = null)
        {
            catService ??= new Mock<ICatService>().Object;
            fileService ??= new Mock<IFileService>().Object;
            logger ??= new Mock<ILogger<CatsControllerV2>>().Object;

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(m => m.ContentRootPath).Returns("/");

            settings ??= _serviceProvider.GetService<IOptions<ApplicationOptions>>();
            responseMessage.Headers.Add("x-inlinecount", "10");

            logger ??= new Mock<ILogger<CatsControllerV2>>().Object;
            return new CatsControllerV2(catService, fileService, logger, settings);
        }

    }
}
