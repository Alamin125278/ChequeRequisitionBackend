using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.LocalFileUpload;

public class LocalFileUploadEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/local-file-upload/bulk", async (BulkLocalFileUploadCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            var response = new ResponseDto<LocalFileUploadResult>
            {
                Message = result.IsSuccess ? "Successfully uploaded cheque items." : "Failed to upload cheque items.",
                Data = result,
                StatusCode = StatusCodes.Status200OK 
            };

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .Accepts<BulkLocalFileUploadCommand>("application/json")
        .RequireAuthorization()
        .WithName("BulkLocalFileUpload")
        .Produces<ResponseDto<LocalFileUploadResult>>(StatusCodes.Status200OK)
        .WithTags("LocalFileUpload");
    }
}
