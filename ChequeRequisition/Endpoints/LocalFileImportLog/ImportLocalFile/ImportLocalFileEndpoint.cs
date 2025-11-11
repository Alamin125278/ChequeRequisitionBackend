using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.LocalFileImportLog.ImportLocalFile
{
    public class ImportLocalFileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/local-file-import-log/import", async (ImportLocalFileCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                try { 
                var result = await sender.Send(command, cancellationToken);
                    return Results.Ok(new ResponseDto<ImportLocalFileRes>
                    { 
                        Success = true,
                        Message = "Local file imported successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });

                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while importing the local file.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).Accepts<ImportLocalFileCommand>("application/json")
            .WithName("ImportLocalFile")
            .RequireAuthorization()
            .Produces<ImportLocalFileRes>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("LocalFileImportLog");
        }
    }
}
