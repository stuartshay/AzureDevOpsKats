using AzureDevOpsKats.Service.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.IO;

namespace AzureDevOpsKats.Web.Extensions
{
    /// <summary>
    /// Service Collection Extensions
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        ///  Display Configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public static void DisplayConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var config = configuration.Get<ApplicationOptions>();
            Console.WriteLine($"Environment: {environment.EnvironmentName}");
            Console.WriteLine($"CurrentDirectory: {Directory.GetCurrentDirectory()}");

            Console.WriteLine($"FilePath: {config.FileStorage.FilePath}");
            Console.WriteLine($"RequestPath: {config.FileStorage.RequestPath}");
            Console.WriteLine($"PhysicalFilePath: {config.FileStorage.PhysicalFilePath}");

            Console.WriteLine($"DbConnection: {config.ConnectionStrings.DbConnection}");
            Console.WriteLine($"DataProtection: {config.Dataprotection.Enabled}");
            Console.WriteLine($"RedisConnection: {config.Dataprotection.RedisConnection}");
            Console.WriteLine($"Redis Key: {config.Dataprotection.RedisKey}");

            var elasticUrl = configuration.GetValue<string>("Logging:ElasticSearchConfiguration:ElasticUrl");
            var elasticEnabled = configuration.GetValue<bool>("Logging:ElasticSearchConfiguration:ElasticEnabled");
            Console.WriteLine($"Elastic Uri: {elasticUrl}");
            Console.WriteLine($"Elastic Enabled: {elasticEnabled}");
            Console.WriteLine("MemoryHealth - Healthy:{0}", config.MemoryHealthConfiguration.Healthy);
        }

        public static IServiceCollection AddCustomDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<ApplicationOptions>();

            Console.WriteLine($"Data Protection Enabled:{config.Dataprotection.Enabled}");
            if (config.Dataprotection.Enabled)
            {
                var redis = ConnectionMultiplexer.Connect(config.Dataprotection.RedisConnection);
                services.AddDataProtection()
                    .SetApplicationName(config.Dataprotection.RedisKey)
                    .PersistKeysToStackExchangeRedis(redis, config.Dataprotection.RedisKey);
            }

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                //options.OperationFilter<SwaggerDefaultValues>();
                //options.SwaggerDoc("v1", new OpenApiInfo
                //{
                //    Title = "AzureDevOpsKats.Web",
                //    Version = "v1",
                //    Description = "AzureDevOpsKats.Web",
                //    Contact = new OpenApiContact()
                //    {
                //        Email = "stuartshay@yahoo.com",
                //        Name = "Stuart Shay",
                //        Url = new Uri("https://github.com/stuartshay"),
                //    },
                //});

                options.IncludeXmlComments(GetXmlCommentsPath());
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            return services;
        }

        public static IServiceCollection AddCustomCookiePolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            return services;
        }

        public static IServiceCollection AddApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            return services;
        }

        /// <summary>
        /// Api Versioning
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddApiVersioning(options => { options.ReportApiVersions = true; });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        private static string GetXmlCommentsPath()
        {
            var basePath = AppContext.BaseDirectory;
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
            var fileName = Path.GetFileName(assemblyName + ".xml");

            return Path.Combine(basePath, fileName);
        }
    }
}
