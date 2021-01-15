﻿using System;
using System.Data.Common;
using System.IO;
using AutoMapper;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<ApplicationOptions>(builder)
                .AddSingleton(builder)
                .AddSingleton<ICatRepository>(provider => new CatRepository(dbConnection, provider.GetService<ILogger<CatRepository>>()))
                .AddScoped<ICatService, CatService>()
                .AddSingleton(mapper)
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
