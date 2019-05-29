using System.Collections.Generic;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Test.Fixture;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Net.Http;
using AzureDevOpsKats.Test.Helpers;
using System.Net;

namespace AzureDevOpsKats.Test.Mock
{
    public class CatsControllerV2MockTest : IClassFixture<CatConfigurationFixture>
    {
        [Fact(Skip ="Fix")]
        [Trait("Category", "Mock")]
        public void Get_Cats_Paging_List_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCount()).Returns(2);
            mockCatService.Setup(b => b.GetCats())
                .Returns(new List<CatModel>()
                {
                    new CatModel{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new CatModel{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetCatsControllerV2(httpResponse, mockCatService.Object);

            var sut = controller.Get(2, 1);
            controller.ControllerContext = MockHelpers.GetHttpContext();

            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            var objectResult = sut as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var cats = objectResult.Value as List<CatModel>;
            Assert.NotNull(cats);
            Assert.NotEmpty(cats);
            Assert.True(cats.Count == 2);
            Assert.Equal(1, cats[0].Id);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cats_Paging_List_NotFound()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCount()).Returns(0);

            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.NotFound);
            var controller = GetCatsControllerV2(httpResponse, mockCatService.Object);

            var sut = controller.Get(2, 1);

            Assert.NotNull(sut);
            Assert.IsType<NotFoundResult>(sut);
        }

        private CatsControllerV2 GetCatsControllerV2(HttpResponseMessage responseMessage, ICatService catService = null, ILogger<CatsControllerV2> logger = null)
        {
            catService = catService ?? new Mock<ICatService>().Object;

            // TODO - Add to Helper
            responseMessage.Headers.Add("x-inlinecount", "10");

            logger = logger ?? new Mock<ILogger<CatsControllerV2>>().Object;
            return new CatsControllerV2(catService, logger);
        }

    }
}
