using AzureDevOpsKats.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureDevOpsKats.Test.DataSource
{
    public class AzureDevOpsKatsContext : DbContext
    {
        public AzureDevOpsKatsContext()
        { }

        public AzureDevOpsKatsContext(DbContextOptions<AzureDevOpsKatsContext> options)
            : base(options)
        { }

        public DbSet<Cat> Cats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>();
        }
    }
}
