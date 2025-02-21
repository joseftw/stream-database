namespace JOS.ExampleWeb;

public class RealEstateReadModel
{
    public required Guid Id { get; init; }
    public required IReadOnlyCollection<string> ImageIds { get; init; }
}
