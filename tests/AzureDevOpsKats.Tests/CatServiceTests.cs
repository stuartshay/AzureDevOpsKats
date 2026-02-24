using AzureDevOpsKats.Web.Data;
using AzureDevOpsKats.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace AzureDevOpsKats.Tests;

public class CatServiceTests : IDisposable
{
    private readonly CatDbContext _db;
    private readonly CatService _service;
    private readonly string _tempDir;

    public CatServiceTests()
    {
        var options = new DbContextOptionsBuilder<CatDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new CatDbContext(options);
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);

        var webRoot = Path.Combine(_tempDir, "wwwroot");
        Directory.CreateDirectory(webRoot);

        var env = new TestWebHostEnvironment(webRoot);
        var logger = NullLogger<CatService>.Instance;
        _service = new CatService(_db, env, logger);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoCats()
    {
        var result = await _service.GetAllAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_AddsCat()
    {
        var cat = await _service.CreateAsync("Whiskers", "A fluffy cat", null);

        cat.Id.Should().BeGreaterThan(0);
        cat.Name.Should().Be("Whiskers");
        cat.Description.Should().Be("A fluffy cat");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCat()
    {
        var created = await _service.CreateAsync("Mittens", "Black cat", null);
        var found = await _service.GetByIdAsync(created.Id);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Mittens");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesCat()
    {
        var cat = await _service.CreateAsync("OldName", "OldDesc", null);
        var updated = await _service.UpdateAsync(cat.Id, "NewName", "NewDesc", null);

        updated.Should().NotBeNull();
        updated!.Name.Should().Be("NewName");
        updated.Description.Should().Be("NewDesc");
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.UpdateAsync(999, "Name", "Desc", null);
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_RemovesCat()
    {
        var cat = await _service.CreateAsync("ToDelete", "Bye", null);
        var deleted = await _service.DeleteAsync(cat.Id);

        deleted.Should().BeTrue();
        (await _service.GetByIdAsync(cat.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.DeleteAsync(999);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOrderedByName()
    {
        await _service.CreateAsync("Zebra", "Z cat", null);
        await _service.CreateAsync("Alpha", "A cat", null);
        await _service.CreateAsync("Mittens", "M cat", null);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha");
        result[1].Name.Should().Be("Mittens");
        result[2].Name.Should().Be("Zebra");
    }

    public void Dispose()
    {
        _db.Dispose();
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        GC.SuppressFinalize(this);
    }

    private sealed class TestWebHostEnvironment(string webRootPath) : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = webRootPath;
        public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = null!;
        public string ApplicationName { get; set; } = "Tests";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = webRootPath;
        public string EnvironmentName { get; set; } = "Testing";
    }
}
