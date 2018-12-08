using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var catService = GetCatService(mockCatRepository.Object);

            //Act 
            var results = catService.GetCats().ToList();

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
            // Arrange 
            int recordCount = 1000;



            var mockCatRepository = new Mock<ICatRepository>();
            mockCatRepository.Setup(b => b.GetCats())
                .Returns(CatDataSet.GetCatTableData(recordCount));

            var catService = GetCatService(mockCatRepository.Object);

            //Act 
            var results = catService.GetCats().ToList();


            //Assert
            Assert.NotNull(results);
            Assert.IsType<List<CatModel>>(results);
            Assert.NotEmpty(results);

            var cat1 = results.Single(c => c.Id == 1);

            _output.WriteLine(cat1.Name);
            //Assert.Equal("My Cat 1", cat1.Description);
        }


        private ICatService GetCatService(ICatRepository catRepository = null)
        {
            catRepository = catRepository ?? new Mock<ICatRepository>().Object;

            AutoMapperConfiguration.Configure();

            return new CatService(catRepository);
        }

    }
}
