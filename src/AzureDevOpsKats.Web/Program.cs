using AzureDevOpsKats.Web.Components;
using AzureDevOpsKats.Web.Data;
using AzureDevOpsKats.Web.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<CatDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=Data/cats.sqlite"));

builder.Services.AddScoped<ICatService, CatService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatDbContext>("database");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSerilogRequestLogging();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(AzureDevOpsKats.Client._Imports).Assembly);

app.Run();

public partial class Program;
