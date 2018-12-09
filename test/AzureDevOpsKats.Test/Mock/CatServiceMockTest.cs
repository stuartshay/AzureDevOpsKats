using System.Collections.Generic;
using System.Linq;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Mappings;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Service.Service;
using Xunit;
using Moq;
using Xunit.Abstractions;

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
        public void Get_Cats_List_ReturnsData()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCats())
                .Returns(new List<Cat>()
                {
                    new Cat{Id = 1, Description = "My Cat 1", Name = "Cat 1", Photo = "MyPhoto 1"},
                    new Cat{Id = 2, Description = "My Cat 2", Name = "Cat 2", Photo = "MyPhoto 2"}
                });

            var sut = GetCatService(mockCatRepository.Object);

            //Act 
            var results = sut.GetCats().ToList();

            //Assert
            Assert.NotNull(results);
            Assert.IsType<List<CatModel>>(results);
            Assert.NotEmpty(results);

            var cat1 = results.Single(c => c.Id == 1);
            Assert.Equal("My Cat 1", cat1.Description);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cats_Generated_List_ReturnsData()
        {
            int recordCount = 1000;

            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCats())
                .Returns(CatDataSet.GetCatTableData(recordCount));

            var sut = GetCatService(mockCatRepository.Object);

            //Act 
            var results = sut.GetCats().ToList();

            //Assert
            Assert.NotNull(results);
            Assert.IsType<List<CatModel>>(results);
            Assert.NotEmpty(results);
            Assert.Equal(recordCount, results.Count);

            var cat1 = results.Single(c => c.Id == 1);
            _output.WriteLine(cat1.Name);

            Assert.NotNull(cat1.Name);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Get_Cat_ReturnsData()
        {
            var description = "My Cat 1";
            var name = "Cat";

            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCat(1))
                .Returns(new Cat { Id = 1, Description = description, Name = name, Photo = "MyPhoto 1" });

            var sut = GetCatService(mockCatRepository.Object);

            //Act 
            var result = sut.GetCat(1);
            Assert.NotNull(result);
            Assert.IsType<CatModel>(result);
            Assert.NotNull(result.Name);

            Assert.Equal(name, result.Name);
            Assert.Equal(description, result.Description);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Create_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.CreateCat(It.IsAny<Cat>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);
            var cat = new CatModel { Id = 1, Description = "Cat", Name = "Cat", Photo = "myphoto.jpg" };

            //Act 
            sut.CreateCat(cat);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Update_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.EditCat(It.IsAny<Cat>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);
            var cat = new CatUpdateModel { Description = "Cat", Name = "Cat" };

            //Act 
            sut.EditCat(1, cat);
        }

        [Fact]
        [Trait("Category", "Mock")]
        public void Delete_Cat()
        {
            // Arrange 
            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(mr => mr.DeleteCat(It.IsAny<int>()))
                .Verifiable();

            var sut = GetCatService(mockCatRepository.Object);

            //Act 
            sut.DeleteCat(1);
        }

        private ICatService GetCatService(ICatRepository catRepository = null)
        {
            catRepository = catRepository ?? new Mock<ICatRepository>().Object;

            AutoMapperConfiguration.Configure();

            return new CatService(catRepository);
        }
    }
}
