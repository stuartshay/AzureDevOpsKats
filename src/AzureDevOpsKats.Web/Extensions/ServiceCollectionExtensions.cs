using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AzureDevOpsKats.Web
{
    /// <summary>
    /// Service Collection Extensions
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AzureDevOpsKats.Web",
                    Version = "v1",
                    Description = "AzureDevOpsKats.Web",
                    Contact = new OpenApiContact()
                    {
                        Email = "stuartshay@yahoo.com",
                        Name = "Stuart Shay",
                        Url = new Uri("https://github.com/stuartshay"),
                    },
                });

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

        private static string GetXmlCommentsPath()
        {
            var basePath = AppContext.BaseDirectory;
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var fileName = Path.GetFileName(assemblyName + ".xml");

            return Path.Combine(basePath, fileName);
        }
    }
}
