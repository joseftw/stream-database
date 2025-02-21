using JOS.Result;
using Npgsql;
using System.Data;

namespace JOS.ExampleWeb;

public class RealEstateImageQueryHandler
{
    private readonly NpgsqlConnection _dbConnection;

    public RealEstateImageQueryHandler(NpgsqlConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    public async Task<Result<ReadRealEstateImageModel>> Handle(Guid realEstateId, Guid imageId)
    {
        const string sql =
            """
            SELECT id, data, metadata->>'MimeType' as mimeType FROM real_estate_images
            WHERE id = @imageId AND real_estate_id = @realEstateId
            """;
        await _dbConnection.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, _dbConnection);
        cmd.Parameters.Add(new() { ParameterName = "realEstateId", Value = realEstateId });
        cmd.Parameters.Add(new() { ParameterName = "imageId", Value = imageId });

        var reader = await cmd.ExecuteReaderAsync();
        if(!reader.HasRows)
        {
            return new FailedResult<ReadRealEstateImageModel>(new Error("NotFound", "Image was not found"));
        }

        await reader.ReadAsync();
        var id = reader.GetGuid("id");
        var mimeType = reader.GetString("mimeType");
        var data = await reader.GetStreamAsync(reader.GetOrdinal("data"));
        return new SucceededResult<ReadRealEstateImageModel>(new ReadRealEstateImageModel
        {
            Id = id, Data = data, MimeType = mimeType
        });
    }
}
