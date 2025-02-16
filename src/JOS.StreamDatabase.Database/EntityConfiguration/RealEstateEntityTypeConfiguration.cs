using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JOS.StreamDatabase.Database.EntityConfiguration;

internal class RealEstateEntityTypeConfiguration : IEntityTypeConfiguration<RealEstate>
{
    public void Configure(EntityTypeBuilder<RealEstate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Ignore(x => x.Images);
        builder.HasMany<RealEstateImage>("_images").WithOne();
        builder.Navigation("_images").AutoInclude();
    }
}
