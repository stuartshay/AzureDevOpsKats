using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Serilog;

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
        ///  Main Entry Point.
        /// </summary>
        /// <param name="args">Main Arguments</param>
        /// <returns>Start up status</returns>
        public static int Main(string[] args)
        {
            // https://www.vivienfabing.com/aspnetcore/2019/02/21/how-to-add-logging-on-azure-with-aspnetcore-and-serilog.html
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AzureAnalytics(workspaceId: "8a8cc6bf-50af-4ad6-abb7-41ce59795f7a","JN168nKopk0YJgGmU/IdrvWqzVtNw+E6eosu9i5QULiBDbqo8ifX/+0szfEONH7ZvP7XyY7OsfhdHsmVhDNZxg==")
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            try
            {
                Log.Information("Init:AzureDevOpsKats.Web");
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Create WebHost Builder.
        /// </summary>
        /// <param name="args">WebHostBuilder Arguments</param>
        /// <returns>IWebHost Builder</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights() // Enable Application Insights
                .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                     logging.AddDebug();

                     logging.AddApplicationInsights("tkjrca4geosn58b5jq0m1nsfg3o2oz8jxv58gsys");
                     logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                 })
                .UseSerilog();
    }
}
