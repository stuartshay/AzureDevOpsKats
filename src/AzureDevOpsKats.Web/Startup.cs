using System.IO;
using AutoMapper;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using AzureDevOpsKats.Web.Extensions;
using AzureDevOpsKats.Web.Extensions.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            services.Configure<ApplicationOptions>(Configuration);
            services.AddSingleton(Configuration);

            services.DisplayConfiguration(Configuration, Environment);
            var config = Configuration.Get<ApplicationOptions>();
            
            string connection = $"Data Source={Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), config.ConnectionStrings.DbConnection ))};";
            string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath);

            // Services Configuration
            services.AddApiBehaviorOptions();

            services.AddApiVersioning(Configuration);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddCustomSwagger(Configuration);

            services.AddCustomDataProtection(Configuration);
            services.AddCustomCors(Configuration);
            services.AddCustomCookiePolicy(Configuration);

            // Application Services 
            services.AddSingleton<ICatRepository>(new CatRepository(connection));
            services.AddScoped<ICatService, CatService>();
            services.AddScoped<IFileService, FileService>();

            // Mapping Configuration
            var mapperConfig = new MapperConfiguration(cfg => { cfg.AddMaps("AzureDevOpsKats.Service"); });
            IMapper mapper = new Mapper(mapperConfig);
            services.AddSingleton(mapper);

            services.AddRazorPages();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
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
}
