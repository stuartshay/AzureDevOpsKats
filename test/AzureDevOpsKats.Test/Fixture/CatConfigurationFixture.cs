using System.IO;
using AzureDevOpsKats.Service.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Test.Fixture
{
    public class CatConfigurationFixture
    {
        public ServiceProvider ServiceProvider { get; set; }

        public CatConfigurationFixture()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<ApplicationOptions>(builder);
            services.AddSingleton(builder);
            services.AddTransient(
                provider => Options.Create(new ApplicationOptions
                {
                    FileStorage = new FileStorage { FilePath = "XXXX", RequestPath = "XXXX", },
                }));

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
