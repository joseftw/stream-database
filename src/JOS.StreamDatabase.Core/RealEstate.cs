using JOS.Result;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JOS.StreamDatabase.Core;

public class RealEstate : Entity<Guid>
{
    private List<RealEstateImage> _images = null!;

    public const int NameMaxLength = 255;
    public required string Name { get; init; }
    public IReadOnlyCollection<RealEstateImage> Images => _images;

    public void AddImage(RealEstateImage image)
    {
        _images.Add(image);
    }

    public Result.Result RemoveImage(RealEstateImage image)
    {
        var result = _images.Remove(image);
        // TODO better error
        return result ? new SucceededResult() : new FailedResult(new Error("Remove", "Image was not removed"));
    }

    public static Result<RealEstate> Create(
        Guid id, string name, IReadOnlyCollection<RealEstateImage>? images = null)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            return new FailedResult<RealEstate>(new NullOrEmptyError(nameof(name)));
        }

        if(name.Length > NameMaxLength)
        {
            return new FailedResult<RealEstate>(new ValueTooLongError(NameMaxLength, name.Length, name));
        }

        return new SucceededResult<RealEstate>(new RealEstate
        {
            Id = id,
            _images = images?.ToList() ?? [],
            Name = name
        });
    }
}
