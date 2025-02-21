using Microsoft.AspNetCore.Mvc;

namespace JOS.ExampleWeb;

public static class RealEstateImageEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid realEstateId, [FromRoute] Guid imageId, [FromServices] RealEstateImageQueryHandler handler)
    {
        var result = await handler.Handle(realEstateId, imageId);
        if(result.Failed)
        {
            return result.Error.ErrorType switch
            {
                "NotFound" => Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                    detail: result.Error.ErrorMessage),
                _ => Results.InternalServerError("Something bad happened...")
            };
        }

        return Results.Stream(result.Data.Data, result.Data.MimeType);
    }
}
