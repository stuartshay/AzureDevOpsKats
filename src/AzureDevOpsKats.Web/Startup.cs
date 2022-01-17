using System;
using System.IO;
using AutoMapper;
using AzureDevOpsKats.Common.Configuration;
using AzureDevOpsKats.Common.Constants;
using AzureDevOpsKats.Common.HealthChecks;
using AzureDevOpsKats.Common.HealthChecks.Extensions;
using AzureDevOpsKats.Common.Logging;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using AzureDevOpsKats.Web.Extensions;
using AzureDevOpsKats.Web.Extensions.Swagger;
using AzureDevOpsKats.Web.HostedServices;
using Elastic.Apm.NetCoreAll;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using ApplicationOptions = AzureDevOpsKats.Service.Configuration.ApplicationOptions;

namespace AzureDevOpsKats.Web
{
    /// <summary>
    ///
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///  Startup
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        ///
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///
        /// </summary>
        public IWebHostEnvironment Environment { get; }



        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddOptions();
            services.Configure<Common.Configuration.ApplicationOptions>(Configuration);
            services.Configure<ApplicationOptions>(Configuration);
            services.Configure<SmtpConfiguration>(Configuration.GetSection(SmtpConfiguration.SectionName));
            services.AddSingleton(Configuration);

            var startupHealthCheck = new StartupTasksHealthCheck();
            services.AddSingleton(startupHealthCheck);

            services.DisplayConfiguration(Configuration, Environment);
            var config = Configuration.Get<ApplicationOptions>();
            var commonConfig = Configuration.Get<Common.Configuration.ApplicationOptions>();
            var smtp = Configuration.Get<SmtpConfiguration>();

            string connection = $"Data Source={Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), config.ConnectionStrings.DbConnection))};";
            string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath).Replace('\\', '/'); ;

            //Logging
            services.AddTransient<LoggingDelegatingHandler>();

            // Services Configuration
            services.AddApiBehaviorOptions();

            services.AddApiVersioning(Configuration);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddCustomSwagger(Configuration);

            services.AddCustomDataProtection(Configuration);
            services.AddCustomCors(Configuration);
            services.AddCustomCookiePolicy(Configuration);

            // Application Services
            services.AddSingleton<ICatRepository>(provider =>
                new CatRepository(connection, provider.GetService<ILogger<CatRepository>>()));

            services.AddScoped<ICatService, CatService>();
            services.AddScoped<IFileService, FileService>();

            // Mapping Configuration
            var mapperConfig = new MapperConfiguration(cfg => { cfg.AddMaps("AzureDevOpsKats.Service"); });
            IMapper mapper = new Mapper(mapperConfig);
            services.AddSingleton(mapper);

            services.AddRazorPages();

            //Scoped Services
            services.AddScoped<ICatsHostedService, CatsHostedService>();

            services.AddCronJob<MyCronJob1>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/5 * * * *";
            });

            // MyCronJob2 calls the scoped service CatsHostedService
            services.AddCronJob<MyCronJob2>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"* * * * *";
            });

            services.AddCronJob<MyCronJob3>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"50 12 * * *";
            });


            //Health Checks
            services
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.AddHealthCheckEndpoint("health", "http://localhost:5000/health");
                    setup.AddHealthCheckEndpoint("health-infra", "http://localhost:5000/health-infra");
                    setup.AddHealthCheckEndpoint("health-system", "http://localhost:5000/health-system");
                })
                .AddInMemoryStorage()
                .Services
                .AddCustomHealthCheck(Configuration)
                .AddHealthChecks()
                .AddElasticSearchHealthCheck(commonConfig.ElasticSearchConfiguration)
                .AddCheck(name: "SQLite Database", new SqliteConnectionHealthCheck(connectionString: connection, testQuery: "Select 1"),
                   failureStatus: HealthStatus.Unhealthy, tags: new string[] { HealthCheckType.Database.ToString(), HealthCheckType.Infrastructure.ToString() })
                .Services
                .AddControllers();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            startupHealthCheck.StartupTaskCompleted = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="apiVersionDescriptionProvider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            ConfigureSwagger(app, apiVersionDescriptionProvider);
            app.UseMiddleware(typeof(HttpHeaderMiddleware));

            //app.UseAllElasticApm(Configuration);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                endpoints.MapHealthChecks("health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("health-infra", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains(HealthCheckType.Infrastructure.ToString()),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("health-system", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains(HealthCheckType.System.ToString()),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                // Health Check UI Configuration
                endpoints.MapHealthChecksUI(setup =>
                {
                    setup.UIPath = "/health-ui";
                    setup.ApiPath = "/health-ui-api";
                    setup.AddCustomStylesheet("dotnet.css");
                });
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        private static void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"AzureDevOpsKats.Web - {description.GroupName.ToUpperInvariant()}");
                }
            });
        }
    }
}
