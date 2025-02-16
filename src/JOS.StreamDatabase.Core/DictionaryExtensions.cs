using JOS.Result;
using System;
using System.Collections.Generic;
using System.IO;

namespace JOS.StreamDatabase.Core;

internal static class DictionaryExtensions
{
    internal static Result<(string MimeType, string Filename, string Extension)> ExtractBaseMetadata(
        this Dictionary<string, object> metadata)
    {
        if(!metadata.Comparer.Equals(StringComparer.OrdinalIgnoreCase))
        {
            metadata = new Dictionary<string, object>(metadata, StringComparer.OrdinalIgnoreCase);
        }

        var getMimeTypeResult = GetMimeType(metadata);
        if(getMimeTypeResult.Failed)
        {
            return new FailedResult<(string MimeType, string Filename, string Extension)>(getMimeTypeResult.Error!);
        }

        var getFilenameResult = GetFilename(metadata);
        if(getFilenameResult.Failed)
        {
            return new FailedResult<(string MimeType, string Filename, string Extension)>(getFilenameResult.Error!);
        }
        var extension = Path.GetExtension(getFilenameResult.Data.AsSpan()).TrimStart('.').ToString();

        return new SucceededResult<(string MimeType, string Filename, string Extension)>((
            getMimeTypeResult.Data,
            getFilenameResult.Data,
            extension));
    }

    private static Result<string> GetMimeType(Dictionary<string, object> metadata)
    {
        return GetStringProperty(nameof(FileMetadata.MimeType), metadata);
    }

    private static Result<string> GetFilename(Dictionary<string, object> metadata)
    {
        var getFileNameResult = GetStringProperty(nameof(FileMetadata.Filename), metadata);
        if(getFileNameResult.Failed)
        {
            return getFileNameResult;
        }

        return !Path.HasExtension(getFileNameResult.Data)
            ? new FailedResult<string>(new ValidationError("The filename needs to have an extension"))
            : getFileNameResult;
    }

    private static Result<string> GetStringProperty(string name, Dictionary<string, object> metadata)
    {
        if(metadata.TryGetValue(name, out var result))
        {
            if(result is string value)
            {
                return new SucceededResult<string>(value);
            }
        }

        return new FailedResult<string>(new ValidationError($"Failed to extract {name} from metadata"));
    }
}
