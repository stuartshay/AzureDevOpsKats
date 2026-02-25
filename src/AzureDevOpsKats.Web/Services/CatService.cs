using AzureDevOpsKats.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureDevOpsKats.Web.Services;

public class CatService(CatDbContext db, IWebHostEnvironment env, ILogger<CatService> logger) : ICatService
{
    private readonly string _uploadPath = Path.Combine(env.WebRootPath, "uploads");

    public async Task<List<Cat>> GetAllAsync()
    {
        return await db.Cats.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Cat?> GetByIdAsync(long id)
    {
        return await db.Cats.FindAsync(id);
    }

    public async Task<Cat> CreateAsync(string name, string description, IFormFile? photo)
    {
        var cat = new Cat { Name = name, Description = description };

        if (photo is not null)
        {
            cat.Photo = await SavePhotoAsync(photo);
        }

        db.Cats.Add(cat);
        await db.SaveChangesAsync();
        logger.LogInformation("Created cat {Id}: {Name}", cat.Id, cat.Name);
        return cat;
    }

    public async Task<Cat?> UpdateAsync(long id, string name, string description, IFormFile? photo)
    {
        var cat = await db.Cats.FindAsync(id);
        if (cat is null) return null;

        cat.Name = name;
        cat.Description = description;

        if (photo is not null)
        {
            DeletePhoto(cat.Photo);
            cat.Photo = await SavePhotoAsync(photo);
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Updated cat {Id}: {Name}", cat.Id, cat.Name);
        return cat;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var cat = await db.Cats.FindAsync(id);
        if (cat is null) return false;

        DeletePhoto(cat.Photo);
        db.Cats.Remove(cat);
        await db.SaveChangesAsync();
        logger.LogInformation("Deleted cat {Id}: {Name}", cat.Id, cat.Name);
        return true;
    }

    private async Task<string> SavePhotoAsync(IFormFile file)
    {
        Directory.CreateDirectory(_uploadPath);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }

    private void DeletePhoto(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var filePath = Path.Combine(_uploadPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
