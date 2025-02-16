using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JOS.StreamDatabase.Database.EntityConfiguration;

internal class RealEstateImageEntityTypeConfiguration : IEntityTypeConfiguration<RealEstateImage>
{
    public void Configure(EntityTypeBuilder<RealEstateImage> builder)
    {
        builder.ConfigureImage();
        builder.Property(x => x.RealEstateId).IsRequired();
    }
}
