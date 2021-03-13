using AutoMapper;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;

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
            var mapperConfig = new MapperConfiguration(cfg => { cfg.AddMaps("AzureDevOpsKats.Service"); });
            IMapper mapper = new Mapper(mapperConfig);

            var services = new ServiceCollection()
                .AddOptions()
                .Configure<ApplicationOptions>(builder)
                .AddSingleton(builder)
                .AddSingleton<ICatRepository>(provider => new CatRepository(dbConnection, provider.GetService<ILogger<CatRepository>>()))
                .AddScoped<ICatService, CatService>()
                .AddScoped<ILogger<CatRepository>, NullLogger<CatRepository>>()
                .AddScoped<ILogger<CatService>, NullLogger<CatService>>()
                .AddSingleton(mapper)
                .BuildServiceProvider();

            services.GetRequiredService<ICatRepository>();
            CatService = services.GetRequiredService<ICatService>();
        }

        public ICatService CatService { get; }

        public void Dispose()
        {
            _catRepository?.Dispose();
        }
    }
}
