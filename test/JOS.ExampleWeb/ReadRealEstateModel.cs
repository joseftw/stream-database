namespace JOS.ExampleWeb;

public class ReadRealEstateModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required IEnumerable<Uri> Images { get; init; }
}
