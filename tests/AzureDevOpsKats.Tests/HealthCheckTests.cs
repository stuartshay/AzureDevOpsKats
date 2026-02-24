using AzureDevOpsKats.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOpsKats.Tests;

public class HealthCheckTests : IClassFixture<HealthCheckTests.TestFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_ReturnsOk()
    {
        var response = await _client.GetAsync("/ready");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Home_ReturnsOk()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    public class TestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<CatDbContext>)
                        || d.ServiceType == typeof(DbContextOptions)
                        || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                    .ToList();
                foreach (var d in descriptors)
                    services.Remove(d);

                services.AddDbContext<CatDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }
    }
}
