namespace JOS.ExampleWeb;

public class ReadRealEstateImageModel
{
    public required Guid Id { get; init; }
    public required Stream Data { get; init; }
    public required string MimeType { get; init; }
}
