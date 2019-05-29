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
using Microsoft.AspNetCore.Hosting;

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
            Action action = () => { new CatsController(catService, null, null, null, null); };

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

            var controller = GetCatsController(mockCatService.Object);

            var sut = controller.Get();

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
        public void Get_Cat_ReturnsData()
        {
            // Arrange 
            var mockCatService = new Mock<ICatService>();
            mockCatService.Setup(b => b.GetCat(1))
                .Returns(new CatModel { Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1" });

            var controller = GetCatsController(mockCatService.Object);

            var sut = controller.GetById(1);

            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            var objectResult = sut as OkObjectResult;
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

            var controller = GetCatsController(mockCatService.Object);

            var sut = controller.GetById(1);

            Assert.NotNull(sut);
            Assert.IsType<NotFoundResult>(sut);
        }

        [Fact(Skip = "Fix Request Path")]
        [Trait("Category", "Mock")]
        public void Create_Cat()
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
                .Returns((long)id);

            var mockFileService = new Mock<IFileService>();
            mockFileService
                .Setup(mr => mr.SaveFile(It.IsAny<string>(), cat.Bytes))
                .Verifiable();

            var controller = GetCatsController(mockCatService.Object, mockFileService.Object);

            // Act
            var sut = controller.Post(cat);

            // Assert
            Assert.IsType<CreatedAtActionResult>(sut);
            Assert.Equal($"Name:{name}|Description:{description}", cat.ToString());

            var objectResult = sut as CreatedAtActionResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 201);

        }

        [Fact()]
        [Trait("Category", "Mock")]
        public void Create_Cat_Invalid_ModelState()
        {
            // Arrange 
            var cat = new CatCreateModel { Name = "Cat", Bytes = CreateSpecialByteArray(7000) };

            var controller = GetCatsController();
            controller.ModelState.AddModelError("Description", "Required");

            //Act
            var sut = controller.Post(cat);

            //Assert
            var badRequestResult = Assert.IsType<UnprocessableEntityObjectResult>(sut);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat_BadRequest()
        {
            // Arrange 
            var dataSet = new CatModel { Id = 1, Description = "Description" };

            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .Returns(dataSet)
               .Verifiable();

            var controller = GetCatsController(mockCatService.Object);
            controller.ModelState.AddModelError("Name", "Required");

            //Act
            var sut = controller.Put(1, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(sut);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat()
        {
            var dataSet = new CatModel { Id = 1, Description = "Description" };

            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.EditCat(It.IsAny<int>(), It.IsAny<CatUpdateModel>()))
               .Verifiable();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .Returns(dataSet)
               .Verifiable();

            var sut = GetCatsController(mockCatService.Object);

            //Act
            var cat = new CatUpdateModel { Name = "Cat", Description = "Cat Description" };
            var result = sut.Put(1, cat);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat_Not_Found()
        {
            var mockCatService = new Mock<ICatService>();
            mockCatService
               .Setup(mr => mr.GetCat(It.IsAny<int>()))
               .Returns((CatModel) null)
               .Verifiable();

            var sut = GetCatsController(mockCatService.Object);

            //Act
            var cat = new CatUpdateModel { Name = "Cat", Description = "Cat Description" };
            var result = sut.Put(1, cat);

            //Assert
            Assert.IsType<NotFoundResult>(result);
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

            var controller = GetCatsController(mockCatService.Object);

            //Act
            var result = controller.Delete(1);

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

            var env = new Mock<IHostingEnvironment>();
            env.Setup(m => m.ContentRootPath).Returns("/");

            settings = settings ?? _serviceProvider.GetService<IOptions<ApplicationOptions>>();

            return new CatsController(catService, fileService, logger, env.Object, settings);
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
