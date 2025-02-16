using JOS.Result;
using System;
using System.IO;

namespace JOS.StreamDatabase.Core;

public class RealEstateImage : ImageFile
{
    private RealEstateImage() {}
    public required Guid RealEstateId { get; init; }

    public static Result<RealEstateImage> Create(
        Guid id, Guid realEstateId, Stream data, ImageMetadata metadata)
    {
        if(id.Equals(Guid.Empty))
        {
            return new FailedResult<RealEstateImage>(new NullOrEmptyError(nameof(id)));
        }

        if(realEstateId.Equals(Guid.Empty))
        {
            return new FailedResult<RealEstateImage>(new NullOrEmptyError(nameof(realEstateId)));
        }

        var image =
            new RealEstateImage { Id = id, Data = data, Metadata = metadata, RealEstateId = realEstateId };
        return new SucceededResult<RealEstateImage>(image);
    }
}
