using System;
using System.IO;
using AzureDevOpsKats.Web;
using AzureDevOpsKats.Web.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AzureDevOpsKats.Test.Mock
{
    public class MainMockTest
    {
        [Fact(Skip = "TODO")]
        [Trait("Category", "Mock")]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            //  Arrange
            // https://stackoverflow.com/questions/47482256/how-to-unit-test-startup-cs-in-net-core
             
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");

            Mock<IConfiguration> configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);

            Mock<ILogger<Startup>> loggerStub = new Mock<ILogger<Startup>>();

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object, loggerStub.Object);
            //var target = new Startup(Configuration, loggerStub.Object);

            //  Act

            target.ConfigureServices(services);
            services.AddTransient<CatsController>();

            //  Assert
            var serviceProvider = services.BuildServiceProvider();

            var controller = serviceProvider.GetService<CatsController>();
            Assert.NotNull(controller);
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
    }
}
