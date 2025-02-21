using Microsoft.AspNetCore.Mvc;

namespace JOS.ExampleWeb;

public static class RealEstateEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid realEstateId, [FromServices]RealEstateQueryHandler realEstateQueryHandler)
    {
        var result = await realEstateQueryHandler.Read(realEstateId);
        if(result.Failed)
        {
            return result.Error.ErrorType switch
            {
                "NotFound" => Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                    detail: result.Error.ErrorMessage),
                _ => Results.InternalServerError("Something bad happened...")
            };
        }

        return Results.Ok(result.Data);
    }
}
