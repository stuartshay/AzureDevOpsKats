using System;
using System.Diagnostics;
using System.IO;
using AzureDevOpsKats.Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
                }).UseSerilog(Logging.ConfigureLogger);


        ///// <summary>
        /////  Main Entry Point.
        ///// </summary>
        ///// <param name="args">Main Arguments</param>
        ///// <returns>Start up status</returns>
        //public static int Main(string[] args)
        //{
        //    Log.Logger = new LoggerConfiguration()
        //        .Enrich.WithMachineName()
        //        .WriteTo.AzureAnalytics(workspaceId: "8a8cc6bf-50af-4ad6-abb7-41ce59795f7a", "JN168nKopk0YJgGmU/IdrvWqzVtNw+E6eosu9i5QULiBDbqo8ifX/+0szfEONH7ZvP7XyY7OsfhdHsmVhDNZxg==")
        //        .WriteTo.ColoredConsole()
        //        .ReadFrom.Configuration(Configuration)
        //        .CreateLogger();

        //    try
        //    {
        //        Log.Information("Init:AzureDevOpsKats.Web");
        //        CreateWebHostBuilder(args).Build().Run();
        //        return 0;
        //    }
        //    catch (Exception)
        //    {
        //        return 1;
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //}

    }
}
