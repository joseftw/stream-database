using JOS.StreamDatabase.Core;
using JOS.StreamDatabase.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Migrator;

public class SeedDatabaseCommand
{
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SeedDatabaseCommand> _logger;

    public SeedDatabaseCommand(
        MyDbContext dbContext,
        ILogger<SeedDatabaseCommand> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public async Task Execute()
    {
        var realEstateId = Guid.Parse("d1ff5e47-12fe-4ff4-921e-c74c93b07739");

        if(await _dbContext.RealEstate.FirstOrDefaultAsync(x => x.Id == realEstateId) == null)
        {
            var realEstateCreateResult = RealEstate.Create(realEstateId, "My house", []);
            _dbContext.RealEstate.Add(realEstateCreateResult.Data);
            await _dbContext.SaveChangesAsync();
        }

        var files = new List<string> { "SeedData.overstevagen.jpg", "SeedData.11mb.jpg", };
        var images = new List<RealEstateImage>();
        foreach(var filename in files)
        {
            var cleanFilename = filename.Replace("SeedData.", string.Empty);
            var existingImage =
                await _dbContext.RealEstateImages.FirstOrDefaultAsync(x => x.Metadata.Filename == cleanFilename);

            if(existingImage != null)
            {
                continue;
            }

            var fileId = Guid.CreateVersion7();
            var data = new EmbeddedResourceQuery().Read<SeedDatabaseCommand>(filename);
            var metadata = ImageMetadata.Create(
                data.Length,
                new Dictionary<string, object>
                {
                    ["filename"] = cleanFilename,
                    ["size"] = data.Length,
                    ["mimeType"] = "image/jpg"
                });
            var imageFileCreateResult = RealEstateImage.Create(fileId, realEstateId, data, metadata.Data);
            if(imageFileCreateResult.Failed)
            {
                throw new Exception($"Failed to create ImageFile. {imageFileCreateResult.Error.ErrorMessage}");
            }

            images.Add(imageFileCreateResult.Data);
        }

        _dbContext.RealEstateImages.AddRange(images);
        await _dbContext.SaveChangesAsync();
    }
}
