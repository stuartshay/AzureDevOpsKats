using System;
using System.IO;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Mappings;
using AzureDevOpsKats.Service.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Swagger;

namespace AzureDevOpsKats.Web
{
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
            var config = Configuration.Get<ApplicationOptions>();

            AutoMapperConfiguration.Configure();

            var dbConnection = Configuration.GetConnectionString("DbConnection");
            string connection = $"Data Source={Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), dbConnection))};";

            string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath);
            services.AddSingleton<IFileService>(new FileService(imagesRoot));

            services.AddSingleton<ICatRepository>(new CatRepository(connection));
            services.AddScoped<ICatService, CatService>();

            // Swagger
            services.AddCustomSwagger(Configuration);

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var config = Configuration.Get<ApplicationOptions>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            ConfigureSwagger(app);
            ConfigureFileBrowser(app);

            app.UseCookiePolicy();
            app.UseMvc();
        }

        private void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureDevOpsKats.Web V1");
            });

        }

        private void ConfigureFileBrowser(IApplicationBuilder app)
        {
            var config = Configuration.Get<ApplicationOptions>();

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), config.FileStorage.FilePath)),
                RequestPath = config.FileStorage.RequestPath
            });

        }
    }

    /// <summary>
    /// 
    /// </summary>
    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "BTIG.Cats.Web",
                    Description = "BTIG.Cats.Web",
                    Version = "v1",
                    TermsOfService = "None",
                });
                options.IncludeXmlComments(GetXmlCommentsPath());
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
