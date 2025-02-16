using JOS.Result;
using System.Collections.Generic;

namespace JOS.StreamDatabase.Core;

public class ImageMetadata : FileMetadata
{
    private ImageMetadata() { }

    public static Result<ImageMetadata> Create(long size, Dictionary<string, object> metadata)
    {
        // ExtractBaseMetadata is just a helper that extracts the
        // extension, filename and mime type from the provided dictionary.

        var extractBaseMetadataResult = metadata.ExtractBaseMetadata();
        if(extractBaseMetadataResult.Failed)
        {
            return new FailedResult<ImageMetadata>(extractBaseMetadataResult.Error!);
        }

        var baseMetadata = extractBaseMetadataResult.Data;
        var imageMetadata = new ImageMetadata
        {
            Extension = baseMetadata.Extension,
            Filename = baseMetadata.Filename,
            MimeType = baseMetadata.MimeType,
            Size = size
        };

        return new SucceededResult<ImageMetadata>(imageMetadata);
    }
}

