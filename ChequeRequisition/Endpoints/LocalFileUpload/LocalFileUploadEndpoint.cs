using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.LocalFileUpload;

public class LocalFileUploadEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/local-file-upload", async (LocalFileUploadCommand command,ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result != null ? Results.Ok() : Results.BadRequest("Failed to upload the file.");

        }).Accepts<LocalFileUploadCommand>("application/json")
        .RequireAuthorization()
         .WithName("LocalFileUpload")
         .Produces<LocalFileUploadResult>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest)
          .WithTags("LocalFileUpload");
    }
}
