using System;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Test.DataSource;
using Microsoft.EntityFrameworkCore;
using CatRepository = AzureDevOpsKats.Test.DataSource.CatRepository;

namespace AzureDevOpsKats.Test.Fixture
{
    public class CatRepositoryInMemoryFixture : IDisposable
    {
        public CatRepositoryInMemoryFixture()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Console.WriteLine("ENV:" + env);

            var builder = new DbContextOptionsBuilder<AzureDevOpsKatsContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            AzureDevOpsKatsContext azureDevOpsKatsContext = new AzureDevOpsKatsContext(builder.Options);

            azureDevOpsKatsContext.Database.EnsureDeleted();
            azureDevOpsKatsContext.Database.EnsureCreated();
            //azureDevOpsKatsContext.EnsureSeedDataForContextRange(150);

            CatRepository = new CatRepository(azureDevOpsKatsContext);
        }

        public string DbConnection { get; private set; }

        public ICatRepository CatRepository { get; private set; }

        public void Dispose()
        {
        }
    }
}
