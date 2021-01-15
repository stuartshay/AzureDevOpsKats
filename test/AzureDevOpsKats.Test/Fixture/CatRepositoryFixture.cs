using System;
using System.IO;
using AzureDevOpsKats.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AzureDevOpsKats.Test.Fixture
{
    public class CatRepositoryFixture : IDisposable
    {
        public CatRepositoryFixture()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            DbConnection = builder.GetConnectionString("DbConnection");

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICatRepository>(provider => new CatRepository(DbConnection, provider.GetService<ILogger<CatRepository>>()))
                .AddScoped<ILogger<CatRepository>, NullLogger<CatRepository>>()
                .BuildServiceProvider();

            CatRepository = serviceProvider.GetRequiredService<ICatRepository>();
        }

        public string DbConnection { get; }

        public ICatRepository CatRepository { get; }

        public void Dispose()
        {
            CatRepository.Dispose();
        }
    }
}
