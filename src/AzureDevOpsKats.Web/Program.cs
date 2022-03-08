using AzureDevOpsKats.Common.Constants;
using AzureDevOpsKats.Common.Logging;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Azure.Services.AppAuthentication;

namespace AzureDevOpsKats.Web
{
    /// <summary>
    ///
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Gets Configuration
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             })
            .ConfigureAppConfiguration((context, builder) =>
            {
                if (context.HostingEnvironment.EnvironmentName == "AwsEcs")
                {
                    var clusterName = Environment.GetEnvironmentVariable("CLUSTER_NAME").ToLower();
                    var systemsManagerReloadSeconds = Convert.ToDouble(Environment.GetEnvironmentVariable("SYSTEMS_MANAGER_RELOAD"));
                    builder.AddSystemsManager($"/{ApplicationConstants.SystemsManagerName}-{clusterName}", optional: false, reloadAfter: TimeSpan.FromSeconds(systemsManagerReloadSeconds));
                }
                if (context.HostingEnvironment.EnvironmentName == "AzureContainer")
                {
                    var keyVaultEndpoint = Environment.GetEnvironmentVariable("KEY_VAULT_ENDPOINT").ToLower();
                    if (!string.IsNullOrEmpty(keyVaultEndpoint))
                    {
                        //https://medium.com/@alexandre.malavasi/secure-secrets-for-net-apps-using-azure-key-vault-and-managed-identities-8c68ae52248e
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        //var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                        //config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                }
            })
            .UseSerilog(Logging.ConfigureLogger);
    }
}
