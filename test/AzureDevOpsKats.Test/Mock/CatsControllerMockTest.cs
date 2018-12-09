using System;
using System.Collections.Generic;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Test.Fixture;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace AzureDevOpsKats.Test.Mock
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
            // Arrange
            ICatService catService = null;

            // Action
            Action action = () => { new CatsController(catService, null, null, null); };

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cats_List_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCats())
                .Returns(new List<CatModel>()
                {
                    new CatModel{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new CatModel{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var sut = GetCatsController(mockCatService.Object);

            var result = sut.Get();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
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
        public void Get_Cat_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(1))
                .Returns(new CatModel { Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1" });

            var sut = GetCatsController(mockCatService.Object);

            var result = sut.Get(1);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var cat = objectResult.Value as CatModel;
            Assert.NotNull(cat);
            Assert.Equal(1, cat.Id);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cat_ReturnsNull()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(It.IsAny<int>()))
                .Returns((CatModel)null);

            var sut = GetCatsController(mockCatService.Object);

            var result = sut.Get(1);

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Create_Cat()
        {
            // Arrange 
            var cat = new CatCreateModel { Name = "Cat", Description = "Cat", Bytes = CreateSpecialByteArray(7000) };
            var catModel = new CatModel { Name = "Cat", Description = "Cat" };
            var mockCatService = new Mock<ICatService>();
            mockCatService
                .Setup(mr => mr.CreateCat(catModel))
                .Verifiable();

            var mockFileService = new Mock<IFileService>();
            mockFileService
                .Setup(mr => mr.SaveFile(It.IsAny<string>(), cat.Bytes))
                .Verifiable();

            var sut = GetCatsController(mockCatService.Object, mockFileService.Object);

            //Act
            var result = sut.Post(cat);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact()]
        [Trait("Category", "Mock")]
        public void Create_Cat_Invalid_ModelState()
        {
            // Arrange 
            var cat = new CatCreateModel { Name = "Cat", Bytes = CreateSpecialByteArray(7000) };
            var sut = GetCatsController();
            sut.ModelState.AddModelError("Description", "Required");

            //Act
            var result = sut.Post(cat);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat_BadRequest()
        {
            // Arrange 
            var sut = GetCatsController();

            //Act
            var result = sut.Put(1, null);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat()
        {
            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.EditCat(It.IsAny<int>(), It.IsAny<CatUpdateModel>()))
               .Verifiable();

            var sut = GetCatsController(mockCatService.Object);

            //Act
            var cat = new CatUpdateModel { Name = "Cat", Description = "Cat Description" };
            var result = sut.Put(1, cat);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Delete_Cat()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(1))
                .Returns(new CatModel { Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1" });
            mockCatService
                .Setup(mr => mr.DeleteCat(It.IsAny<int>()))
                .Verifiable();

            var mockFileService = new Mock<IFileService>();
            mockFileService
                .Setup(mr => mr.DeleteFile(It.IsAny<string>()))
                .Verifiable();

            var sut = GetCatsController(mockCatService.Object, mockFileService.Object);

            //Act
            var result = sut.Delete(1);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Delete_Cat_NotFoundRequest()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(It.IsAny<int>()))
                .Returns((CatModel)null);

            var sut = GetCatsController(mockCatService.Object);

            //Act
            var result = sut.Delete(1);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private CatsController GetCatsController(
            ICatService catService = null,
            IFileService fileService = null,
            ILogger<CatsController> logger = null,
            IOptions<ApplicationOptions> settings = null
            )
        {
            catService = catService ?? new Mock<ICatService>().Object;
            fileService = fileService ?? new Mock<IFileService>().Object;
            logger = logger ?? new Mock<ILogger<CatsController>>().Object;
            settings = settings ?? _serviceProvider.GetService<IOptions<ApplicationOptions>>();

            return new CatsController(catService, fileService, logger, settings);
        }

        private byte[] CreateSpecialByteArray(int length)
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
