using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;

namespace JOS.StreamDatabase.Database;

public class MyDbContext : DbContext
{
    public DbSet<RealEstate> RealEstate { get; set; } = null!;
    public DbSet<RealEstateImage> RealEstateImages { get; set; } = null!;

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
    }
}
