using AWS.Logger;
using AWS.Logger.SeriLog;
using AzureDevOpsKats.Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace AzureDevOpsKats.Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
           (hostingContext, loggerConfiguration) =>
           {
               var env = hostingContext.HostingEnvironment;

               loggerConfiguration.MinimumLevel.Information()
                   .Enrich.FromLogContext()
                   .Enrich.WithExceptionDetails()
                   .Enrich.WithMachineName()
                   .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
                   .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                   .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                   .Enrich.WithExceptionDetails()
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                   .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                   .WriteTo.Console();

               if (hostingContext.HostingEnvironment.IsDevelopment())
               {
                   loggerConfiguration.MinimumLevel.Override(ApplicationConstants.ApplicationName, LogEventLevel.Debug);
               }

               if (hostingContext.HostingEnvironment.EnvironmentName == "AwsEcs")
               {
                   AWSLoggerConfig configuration = new AWSLoggerConfig("devopskats-develop/serilog");
                   configuration.Region = "us-east-1";
                   loggerConfiguration.WriteTo.AWSSeriLog(configuration);
               }

               //var elasticUrl = hostingContext.Configuration.GetValue<string>("Logging:ElasticSearchConfiguration:ElasticUrl");
               //var elasticEnabled = hostingContext.Configuration.GetValue<bool>("Logging:ElasticSearchConfiguration:ElasticEnabled");

                   //if (!string.IsNullOrEmpty(elasticUrl) && elasticEnabled)
                   //{
                   //    loggerConfiguration.WriteTo.Elasticsearch(
                   //        new ElasticsearchSinkOptions(new Uri(elasticUrl))
                   //        {
                   //            AutoRegisterTemplate = true,
                   //            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                   //            IndexFormat = $"{ApplicationConstants.IndexName}-logs-{0:yyyy.MM.dd}",
                   //            MinimumLogEventLevel = LogEventLevel.Debug
                   //        });
                   //}
           };


    }
}
