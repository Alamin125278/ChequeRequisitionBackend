using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChequeRequisiontService.Endpoints.UploadImage
{
    public class UploadImageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/upload-image", async ([FromForm] UplaodImageCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<UplaodImageCommand>("multipart/form-data")
              .Produces<UplaodImageResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("UploadImage")
              .RequireAuthorization()
              .DisableAntiforgery()
              .WithTags("UploadImage")
              .WithDescription("Upload an image to the server.");
        }
    }
}
