namespace JOS.StreamDatabase.Core;

public abstract class ImageFile : File<ImageMetadata>
{
    public override FileType Type { get; init; } = FileType.Image;
}

