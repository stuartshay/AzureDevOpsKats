using System.Collections.Generic;
using AzureDevOpsKats.Data.Entities;
using Bogus;

namespace AzureDevOpsKats.Test.Mock
{
    public static class CatDataSet
    {
        static CatDataSet()
        {
            Faker.GlobalUniqueIndex = 0;
        }

        public static List<Cat> GetCatTableData(int count)
        {
            var catFaker = new Faker<Cat>()
                .RuleFor(c => c.Id, f => f.IndexGlobal)
                .RuleFor(c => c.Name, f => f.Name.FirstName())
                .RuleFor(c => c.Description, f => f.Lorem.Word())
                .RuleFor(c => c.Photo, f => f.Image.LoremPixelUrl());

            var cats = catFaker.Generate(count);
            return cats;
        }
    }
}
