using System.Linq;
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
            this._catService = fixture.CatService;
            this._output = output;
        }

        //[Fact]
        //[Trait("Category", "Intergration")]
        //public void Can_Get_Cat_List()
        //{
        //    var sut = _catService.GetCats();
        //    var count = sut.Count();

        //    Assert.NotNull(sut);
        //    Assert.NotEqual(0, count);
        //}

        //[Fact]
        //[Trait("Category", "Intergration")]
        //public void Can_Get_Cats_Count()
        //{
        //    var sut = _catService.GetCount();
        //    Assert.NotEqual(0, sut);
        //}

        //[Fact]
        //[Trait("Category", "Intergration")]
        //public void Can_Get_Cat_Paging_List()
        //{
        //    var sut = _catService.GetCats(10,1);
        //    var count = sut.Count();

        //    Assert.NotNull(sut);
        //    Assert.NotEqual(0, count);
        //}

        //[Theory]
        //[InlineData(1)]
        //[Trait("Category", "Intergration")]
        //public void Get_Cat(int id)
        //{
        //    var sut = _catService.GetCat(id);

        //    Assert.IsType<CatModel>(sut);
        //    Assert.NotNull(sut);
        //    Assert.Equal(id, sut.Id);

        //    Assert.NotEmpty(sut.Name);
        //    Assert.NotEmpty(sut.Description);
        //    Assert.NotEmpty(sut.Photo);
        //}
    }
}
