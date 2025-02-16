using JOS.Enumeration.Database.EntityFrameworkCore;
using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JOS.StreamDatabase.Database;

internal static class EntityTypeBuilderExtensions
{
    internal static void ConfigureImage<T>(this EntityTypeBuilder<T> builder) where T : ImageFile
    {
        builder.ConfigureFile();
        builder.OwnsOne(x => x.Metadata, b =>
        {
            b.ToJson();
        });
    }

    private static void ConfigureFile<T>(this EntityTypeBuilder<T> builder) where T : File
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).IsRequired().ConfigureEnumeration();
        builder.Property<byte[]>("data").IsRequired();
    }
}
