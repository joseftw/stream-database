using JOS.Result;
using JOS.StreamDatabase.Core;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using File = JOS.StreamDatabase.Core.File;

namespace JOS.StreamDatabase.Database;

internal class PostgresSaveFileCommand : ISaveFileCommand
{
    private readonly NpgsqlConnection _connection;

    public PostgresSaveFileCommand(NpgsqlConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<Result.Result> Execute(File file, Stream data)
    {
        if(!_connection.State.Equals(ConnectionState.Open))
        {
            await _connection.OpenAsync();
        }

        return file switch
        {
            RealEstateImage realEstateImage => await HandleRealEstateImage(realEstateImage, data),
            _ => throw new NotSupportedException($"'{file.GetType().Name}' is not supported")
        };
    }

    private async Task<Result.Result> HandleRealEstateImage(RealEstateImage realEstateImage, Stream data)
    {
        const string sql =
            """
            INSERT INTO real_estate_images (id, real_estate_id, type, metadata, data)
            VALUES (@id, @realEstateId, @type, @metadata, @data)
            """;
        var parameters = new List<NpgsqlParameter>
        {
            new() { ParameterName = "id", Value = realEstateImage.Id },
            new() { ParameterName = "realEstateId", Value = realEstateImage.RealEstateId },
            new() { ParameterName = "type", Value = realEstateImage.Type.Value }
        };

        return await Execute(sql, parameters, realEstateImage, data);
    }

    private async Task<Result.Result> Execute(
        string sql, IReadOnlyCollection<NpgsqlParameter> parameters, File file, Stream data)
    {
        await using var cmd = new NpgsqlCommand(sql, _connection);
        foreach(var parameter in parameters)
        {
            cmd.Parameters.Add(parameter);
        }
        cmd.Parameters.Add(new()
        {
            ParameterName = "metadata",
            NpgsqlDbType = NpgsqlDbType.Jsonb,
            Value = file.GetMetadata()
        });
        cmd.Parameters.Add(new()
        {
            ParameterName = "data",
            Value = data,
            NpgsqlDbType = NpgsqlDbType.Bytea
        });

        try
        {
            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0
                ? new SucceededResult()
                : new FailedResult(new Error("Database", "No rows were inserted"));
        }
        finally
        {
            await data.DisposeAsync();
        }
    }
}
