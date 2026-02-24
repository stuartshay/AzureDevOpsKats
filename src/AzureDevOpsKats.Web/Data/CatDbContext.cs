using Microsoft.EntityFrameworkCore;

namespace AzureDevOpsKats.Web.Data;

public class CatDbContext(DbContextOptions<CatDbContext> options) : DbContext(options)
{
    public DbSet<Cat> Cats => Set<Cat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Photo).HasMaxLength(500);
        });
    }
}
