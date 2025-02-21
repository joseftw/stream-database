using Dapper;
using JOS.Result;
using System.Data;

namespace JOS.ExampleWeb;

public class RealEstateQueryHandler
{
    private readonly IDbConnection _dbConnection;
    private readonly LinkGenerator _linkGenerator;

    public RealEstateQueryHandler(IDbConnection dbConnection, LinkGenerator linkGenerator)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
    }

    public async Task<Result<ReadRealEstateModel>> Read(Guid realEstateId)
    {
        const string sql =
            """
            SELECT r.id, r.name, i.id as image_id
            FROM real_estate r
            LEFT JOIN real_estate_images i ON r.id = i.real_estate_id
            WHERE r.id = @RealEstateId;
            """;

        RealEstateDto? realEstate = null;
        _ = await _dbConnection.QueryAsync<RealEstateDto, Guid?, RealEstateDto>(sql, (realEstateDto, imageId) =>
            {
                realEstate ??= realEstateDto;
                if(imageId.HasValue)
                {
                    realEstate.ImageIds.Add(imageId.Value);
                }

                return realEstate;
            },
            new { RealEstateId = realEstateId },
            splitOn: "image_id"
        );

        var result = realEstate;

        if(result == null)
        {
            var error = new Error("NotFound", $"Real Estate with id {realEstateId} was not found");
            return new FailedResult<ReadRealEstateModel>(error);
        }

        return new SucceededResult<ReadRealEstateModel>(new ReadRealEstateModel
        {
            Id = result.Id,
            Name = result.Name,
            Images = result.ImageIds.Select(x =>
            {
                var path = _linkGenerator.GetPathByName("ReadRealEstateImage",
                    new { realEstateId = realEstateId, imageId = x });
                if(path is null)
                {
                    throw new Exception($"Failed to create link to real estate image file with id {x}");
                }

                return new Uri(path);
            })
        });
    }
}

file class RealEstateDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required List<Guid> ImageIds { get; init; } = [];
}
