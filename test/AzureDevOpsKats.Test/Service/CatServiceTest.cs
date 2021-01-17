using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using AzureDevOpsKats.Test.Fixture;
using Xunit;
using Xunit.Abstractions;

namespace AzureDevOpsKats.Test.Service
{
    public class CatServiceTest : IClassFixture<CatServiceFixture>
    {
        private readonly ICatService _catService;

        private readonly ITestOutputHelper _output;

        public CatServiceTest(CatServiceFixture fixture, ITestOutputHelper output)
        {
            _catService = fixture.CatService;
            _output = output;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Can_Get_Cat_List()
        {
            var sut =  await _catService.GetCats();
            var count = sut.Count();

            Assert.NotNull(sut);
            Assert.NotEqual(0, count);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Can_Get_Cats_Count()
        {
            var sut = await _catService.GetCount();
            Assert.NotEqual(0, sut);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Can_Get_Cat_Paging_List()
        {
            var sut = await _catService.GetCats(10, 1);
            var count = sut.Count();

            Assert.NotNull(sut);
            Assert.NotEqual(0, count);
        }

        [Theory]
        [InlineData(1)]
        [Trait("Category", "Integration")]
        public async Task Get_Cat(int id)
        {
            var sut = await _catService.GetCat(id);

            Assert.IsType<CatModel>(sut);
            Assert.NotNull(sut);
            Assert.Equal(id, sut.Id);

            Assert.NotEmpty(sut.Name);
            Assert.NotEmpty(sut.Description);
            Assert.NotEmpty(sut.Photo);
        }
    }
}
