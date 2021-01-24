using System;
using System.IO;
using System.Net.Http;
using AutoMapper;
using AzureDevOpsKats.Common.Constants;
using AzureDevOpsKats.Common.HealthChecks;
using AzureDevOpsKats.Common.Logging;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using AzureDevOpsKats.Web.Extensions;
using AzureDevOpsKats.Web.Extensions.Swagger;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        public IWebHostEnvironment Environment { get;  }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddOptions();
            services.Configure<AzureDevOpsKats.Common.Configuration.ApplicationOptions>(Configuration);
            services.Configure<AzureDevOpsKats.Service.Configuration.ApplicationOptions>(Configuration);
            services.AddSingleton(Configuration);

            services.DisplayConfiguration(Configuration, Environment);
            var config = Configuration.Get<ApplicationOptions>();
            
            string connection = $"Data Source={Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), config.ConnectionStrings.DbConnection ))};";
            string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath);

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

            //Health Checks
            services
                    
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.AddHealthCheckEndpoint("health", "http://localhost:5000/health");
                    setup.AddHealthCheckEndpoint("health-infa", "http://localhost:5000/health-infa");
                    setup.AddHealthCheckEndpoint("health-system", "http://localhost:5000/health-system");
                    // setup.AddWebhookNotification("webhook1", uri: "http://httpbin.org/status/200?code=ax3rt56s", payload: "{...}");
                })

                .AddInMemoryStorage()
                .Services
                .AddHealthChecks()
                
                .AddUrlGroup(new Uri("http://apm-server:8200"), name: "APM Http", tags: new[] { HealthCheckType.Monitoring.ToString(), "Port:8200" }, 
                    httpMethod: HttpMethod.Get, failureStatus: HealthStatus.Degraded)
                
                .AddUrlGroup(new Uri("http://es01:9200"), name: "ElasticSearch Http", tags: new[] { HealthCheckType.Infrastructure.ToString(), HealthCheckType.Logging.ToString(), "Port:9200" }, 
                    httpMethod: HttpMethod.Get, failureStatus: HealthStatus.Unhealthy)

                // curl -XGET http://kib01:5601/status -I
                .AddUrlGroup(new Uri("http://kib01:5601/status"), name: "Kibana Http", tags: new[] { HealthCheckType.Metrics.ToString(), "Port:5601" },
                    httpMethod: HttpMethod.Head, failureStatus: HealthStatus.Degraded)

                .AddUrlGroup(new Uri("http://traefik:8080/ping"), name: "Traefik Http", tags: new[] { HealthCheckType.Infrastructure.ToString(), "Port:8080" }, 
                    httpMethod: HttpMethod.Get, failureStatus: HealthStatus.Unhealthy)
                
                .AddElasticsearch("http://es01:9200", name: "ElasticSearch Client", failureStatus: HealthStatus.Degraded, 
                    tags: new[] { HealthCheckType.Infrastructure.ToString(), HealthCheckType.Logging.ToString(), "Port:9200" }) 

                .AddRedis("redis", name:"Redis Client", failureStatus: HealthStatus.Degraded, 
                    tags: new[] { HealthCheckType.Infrastructure.ToString(), HealthCheckType.Database.ToString(), "Port:6379" })

                .AddCheck<SystemMemoryHealthCheck>("Memory", tags: new []{HealthCheckType.System.ToString()})
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

            app.UseAllElasticApm(Configuration);

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
                endpoints.MapHealthChecks("health-infa", new HealthCheckOptions()
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

        private void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
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

    // https://www.elastic.co/guide/en/apm/agent/dotnet/current/configuration-on-asp-net-core.html
    //https://github.com/elastic/apm-agent-dotnet/tree/master/sample

}
