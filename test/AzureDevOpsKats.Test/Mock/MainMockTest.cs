﻿using System;
using System.IO;
using AzureDevOpsKats.Web;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AzureDevOpsKats.Test.Mock
{
    public class MainMockTest
    {
        [Fact(Skip = "Fix Env")]
        [Trait("Category", "Mock")]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            //  Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("test");

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(null, Configuration);

            // Act
            target.ConfigureServices(services);
            services.AddTransient<CatsController>();

            var serviceProvider = services.BuildServiceProvider();

            //  Assert


            //var controller = serviceProvider.GetService<CatsController>();
            //Assert.NotNull(controller);
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
    }
}
