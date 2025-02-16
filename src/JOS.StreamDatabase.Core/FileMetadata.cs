namespace JOS.StreamDatabase.Core;

public abstract class FileMetadata
{
    public required long Size { get; init; }
    public required string MimeType { get; init; }
    public required string Filename { get; init; }
    public required string Extension { get; init; }
}

