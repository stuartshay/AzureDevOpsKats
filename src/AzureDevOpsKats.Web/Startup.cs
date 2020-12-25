using System.IO;
using AutoMapper;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using AzureDevOpsKats.Web.Extensions.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration);
            services.AddSingleton(Configuration);

            //services.DisplayConfiguration(Configuration, HostingEnvironment, _logger);
            var config = Configuration.Get<ApplicationOptions>();
            
            string connection = $"Data Source={Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), config.ConnectionStrings.DbConnection ))};";
            string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath);

            // Services Configuration
            services.AddApiBehaviorOptions();

            services.AddApiVersioning(Configuration);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddCustomSwagger(Configuration);

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
        }

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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

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

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);
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
