using System;
using System.Reflection;
using AzureDevOpsKats.Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

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

               var elasticUrl = hostingContext.Configuration.GetValue<string>("Logging:ElasticSearchConfiguration:ElasticUrl");
               var elasticEnabled = hostingContext.Configuration.GetValue<bool>("Logging:ElasticSearchConfiguration:ElasticEnabled");

               //https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker
               var indexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{env.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}";

               if (!string.IsNullOrEmpty(elasticUrl) && elasticEnabled)
               {
                   loggerConfiguration.WriteTo.Elasticsearch(
                       new ElasticsearchSinkOptions(new Uri(elasticUrl))
                       {
                           AutoRegisterTemplate = true,
                           AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                           IndexFormat = $"{ApplicationConstants.IndexName}-logs-{0:yyyy.MM.dd}",
                           MinimumLogEventLevel = LogEventLevel.Debug
                       });
               }
           };
    }
}
