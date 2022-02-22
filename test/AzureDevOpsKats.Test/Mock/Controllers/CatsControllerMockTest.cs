using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Test.Fixture;
using AzureDevOpsKats.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AzureDevOpsKats.Test.Mock.Controllers
{
    public class CatsControllerMockTest : IClassFixture<CatConfigurationFixture>
    {
        private readonly ITestOutputHelper _output;

        private readonly ServiceProvider _serviceProvider;

        public CatsControllerMockTest(CatConfigurationFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public void Given_NullParameter_Constructor_ShouldThrow_ArgumentNullException()
        {
            // Act
            Action action = () =>
            {
                var catsController = new CatsController(null, null, null, null, null);
            };

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cats_List_ReturnsData()
        {
            // Arrange
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCats())
                .ReturnsAsync(new List<CatModel>()
                {
                    new CatModel{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new CatModel{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var sut = await controller.Get();

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut.Result);

            var objectResult = sut.Result as OkObjectResult;
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
        public async Task Get_Cat_ReturnsData()
        {
            // Arrange
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(1))
                .ReturnsAsync(new CatModel { Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1" });

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var sut = await controller.GetById(1);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut.Result);

            var objectResult = sut.Result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var cat = objectResult.Value as CatModel;
            Assert.NotNull(cat);
            Assert.Equal(1, cat.Id);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cat_ReturnsNull()
        {
            // Arrange
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(It.IsAny<int>()))
                .ReturnsAsync((CatModel)null);

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var sut = await controller.GetById(1);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<NotFoundResult>(sut.Result);
        }

        [Fact()]
        [Trait("Category", "Mock")]
        public async Task Create_Cat()
        {
            // Arrange
            var name = "Fun Cat";
            var description = "The Fun Cat";
            var id = 9999;

            var cat = new CatCreateModel { Name = name, Description = description, Bytes = CreateSpecialByteArray(7000) };
            var catModel = new CatModel { Name = "Cat", Description = "Cat" };
            var mockCatService = new Mock<ICatService>();
            mockCatService
                .Setup(mr => mr.CreateCat(catModel))
                .ReturnsAsync(id);

            var mockFileService = new Mock<IFileService>();
            mockFileService
                .Setup(mr => mr.SaveFile(It.IsAny<string>(), cat.Bytes))
                .Verifiable();

            var controller = GetCatsController(mockCatService.Object, mockFileService.Object);

            // Act
            var sut = await controller.Post(cat);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(sut);
            Assert.Equal($"Name:{name}|Description:{description}", cat.ToString());

            var objectResult = sut as CreatedAtRouteResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 201);
        }

        [Fact()]
        [Trait("Category", "Mock")]
        public async Task Create_Cat_Invalid_ModelState()
        {
            // Arrange
            var cat = new CatCreateModel { Name = "Cat", Bytes = CreateSpecialByteArray(7000) };

            var controller = GetCatsController();
            controller.ModelState.AddModelError("Description", "Required");

            //Act
            var sut = await controller.Post(cat);

            //Assert
            var badRequestResult = Assert.IsType<UnprocessableEntityObjectResult>(sut);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Update_Cat_BadRequest()
        {
            // Arrange
            var dataSet = new CatModel { Id = 1, Description = "Description" };

            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .ReturnsAsync(dataSet)
               .Verifiable();

            var controller = GetCatsController(mockCatService.Object);
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var sut = await controller.Put(1, null);

            // Assert
            Assert.IsType<UnprocessableEntityObjectResult>(sut);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Update_Cat()
        {
            var dataSet = new CatModel { Id = 1, Description = "Description" };

            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.EditCat(It.IsAny<int>(), It.IsAny<CatUpdateModel>()))
               .Verifiable();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .ReturnsAsync(dataSet)
               .Verifiable();

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var cat = new CatUpdateModel { Name = "Cat", Description = "Cat Description" };
            var sut = await controller.Put(1, cat);

            // Assert
            Assert.IsType<OkObjectResult>(sut);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Update_Cat_Not_Found()
        {
            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .ReturnsAsync((CatModel)null)
               .Verifiable();

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var cat = new CatUpdateModel { Name = "Cat", Description = "Cat Description" };
            var sut = await controller.Put(1, cat);

            // Assert
            Assert.IsType<NotFoundResult>(sut);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Delete_Cat()
        {
            // Arrange
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(1))
                .ReturnsAsync(new CatModel { Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1" });
            mockCatService
                .Setup(mr => mr.DeleteCat(It.IsAny<int>()))
                .Verifiable();

            var mockFileService = new Mock<IFileService>();
            mockFileService
                .Setup(mr => mr.DeleteFile(It.IsAny<string>()))
                .Verifiable();

            var controller = GetCatsController(mockCatService.Object, mockFileService.Object);

            // Act
            var sut = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(sut);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Delete_Cat_NotFoundRequest()
        {
            // Arrange
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(It.IsAny<int>()))
                .ReturnsAsync((CatModel)null);

            var controller = GetCatsController(mockCatService.Object);

            // Act
            var sut = await controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(sut);
        }

        private CatsController GetCatsController(
            ICatService catService = null,
            IFileService fileService = null,
            ILogger<CatsController> logger = null,
            IOptions<ApplicationOptions> settings = null
            )
        {
            catService ??= new Mock<ICatService>().Object;
            fileService ??= new Mock<IFileService>().Object;
            logger ??= new Mock<ILogger<CatsController>>().Object;

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(m => m.ContentRootPath).Returns("/");

            settings ??= _serviceProvider.GetService<IOptions<ApplicationOptions>>();

            return new CatsController(catService, fileService, logger, env.Object, settings);
        }

        private static byte[] CreateSpecialByteArray(int length)
        {
            var arr = new byte[length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0x20;
            }
            return arr;
        }
    }
}
