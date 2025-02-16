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
        var fileId = Guid.Parse("016b127e-d0d7-48a2-957f-774ffc004772");
        var file = await _dbContext.RealEstateImages.FirstOrDefaultAsync(x => x.Id == fileId);

        if(file is null)
        {
            var metadata = ImageMetadata.Create(112820,
                new Dictionary<string, object>
                {
                    ["filename"] = "overstevagen.jpg", ["size"] = 112818, ["mimeType"] = "image/jpg"
                });

            if(metadata.Failed)
            {
                throw new Exception($"Failed to create metadata for iamge. {metadata.Error.ErrorMessage}");
            }

            var realEstateId = Guid.Parse("d1ff5e47-12fe-4ff4-921e-c74c93b07739");
            var data = new EmbeddedResourceQuery().Read<SeedDatabaseCommand>("SeedData.overstevagen.jpg");
            var imageFileCreateResult = RealEstateImage.Create(fileId, realEstateId, data, metadata.Data);


            if(imageFileCreateResult.Failed)
            {
                throw new Exception($"Failed to create ImageFile. {imageFileCreateResult.Error.ErrorMessage}");
            }
            var realEstateCreateResult = RealEstate.Create(realEstateId, "My house", [imageFileCreateResult.Data]);

            if(realEstateCreateResult.Failed)
            {
                throw new Exception($"Failed to create RealEstate. {realEstateCreateResult.Error.ErrorMessage}");
            }

            _dbContext.RealEstate.Add(realEstateCreateResult.Data);
            _dbContext.RealEstateImages.Add(imageFileCreateResult.Data);
            await _dbContext.SaveChangesAsync();
        }
    }
}
