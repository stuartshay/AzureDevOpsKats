using System;
using System.IO;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Mappings;
using AzureDevOpsKats.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOpsKats.Test.Fixture
{
    public class CatServiceFixture : IDisposable
    {
        private readonly ICatRepository _catRepository;

        public CatServiceFixture()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var dbConnection = builder.GetConnectionString("DbConnection");

            // AutoMapper Configuration
            AutoMapperConfiguration.Configure();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<ApplicationOptions>(builder)
                .AddSingleton(builder)
                .AddSingleton<ICatRepository>(new CatRepository(dbConnection))
                .AddScoped<ICatService, CatService>()
                .BuildServiceProvider();

            serviceProvider.GetRequiredService<ICatRepository>();
            CatService = serviceProvider.GetRequiredService<ICatService>();
        }

        public ICatService CatService { get; }

        public void Dispose()
        {
            _catRepository?.Dispose();
        }
    }
}
