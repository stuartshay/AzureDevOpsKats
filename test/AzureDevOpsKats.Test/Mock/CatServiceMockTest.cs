using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Service.Service;
using Xunit;
using Moq;
using Xunit.Abstractions;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Test.Mock
{
    public class CatServiceMockTest
    {
        private readonly ITestOutputHelper _output;

        public CatServiceMockTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cats_List_ReturnsData()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCats())
                .ReturnsAsync(new List<Cat>()
                {
                    new Cat{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new Cat{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var service = GetCatService(mockCatRepository.Object);

            //Act 
            var results = await service.GetCats();
            var sut = results.ToList();

            //Assert
            Assert.NotNull(sut);
            Assert.IsType<List<CatModel>>(sut);
            Assert.NotEmpty(sut);

            var cat1 = sut.Single(c => c.Id == 1);
            Assert.Equal("My Cat 1", cat1.Description);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cats_Generated_List_ReturnsData()
        {
            int recordCount = 1000;

            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCats())
                .ReturnsAsync(CatDataSet.GetCatTableData(recordCount));

            var service = GetCatService(mockCatRepository.Object);

            //Act 
            var results = await service.GetCats();
            var sut = results.ToList();

            //Assert
            Assert.NotNull(sut);
            Assert.IsType<List<CatModel>>(sut);
            Assert.NotEmpty(sut);
            Assert.Equal(recordCount, sut.Count());

            var cat1 = sut.Single(c => c.Id == 1);
            _output.WriteLine(cat1.Name);

            Assert.NotNull(cat1.Name);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Get_Cat_ReturnsData()
        {
            var description = "My Cat 1";
            var name = "Cat";

            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCat(1))
                .ReturnsAsync(new Cat { Id = 1, Description = description, Name = name, Photo = "MyPhoto 1" });

            var sut = GetCatService(mockCatRepository.Object);

            ////Act 
            var result = await sut.GetCat(1);
            Assert.NotNull(result);
            Assert.IsType<CatModel>(result);
            Assert.NotNull(result.Name);

            Assert.Equal(name, result.Name);
            Assert.Equal(description, result.Description);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Create_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.CreateCat(It.IsAny<Cat>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);
            var cat = new CatModel { Id = 1, Description = "Cat", Name = "Cat", Photo = "myphoto.jpg" };

            //Act 
            await sut.CreateCat(cat);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Update_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.EditCat(It.IsAny<Cat>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);
            var cat = new CatUpdateModel { Description = "Cat", Name = "Cat" };

            //Act 
            await sut.EditCat(1, cat);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public async Task Delete_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.DeleteCat(It.IsAny<int>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);

            //Act 
            await sut.DeleteCat(1);
        }

        private ICatService GetCatService(ICatRepository catRepository = null)
        {
            catRepository ??= new Mock<ICatRepository>().Object;

            ApplicationOptions app = new ApplicationOptions() { FileStorage = new FileStorage { FilePath = "/", RequestPath = "/Images" }};
            var settings = new Mock<IOptions<ApplicationOptions>>();
            settings.Setup(ap => ap.Value).Returns(app);

            //Logger
            var logger = new Mock<ILogger<CatService>>().Object;

            // Mapping Configuration
            var mapperConfig = new MapperConfiguration(cfg => { cfg.AddMaps("AzureDevOpsKats.Service"); });
            IMapper mapper = new Mapper(mapperConfig);

            return new CatService(catRepository, settings.Object, mapper, logger);
        }
    }
}
