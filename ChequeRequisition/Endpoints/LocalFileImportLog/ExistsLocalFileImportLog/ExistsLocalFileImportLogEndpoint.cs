using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
using MediatR;

namespace ChequeRequisiontService.Endpoints.LocalFileImportLog.ExistsLocalFileImportLog
{
    public class ExistsLocalFileImportLogEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/local-file-import-log/exists", async (int bankId, string filename, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new ExistsLocalFileImportLogQuery(bankId, filename);
                    var result = await sender.Send(query, cancellationToken);
                    return Results.Ok(new ResponseDto<ExistsLocalFileImportLogRes>
                    {
                        Success = true,
                        Message = "Consumption report retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while checking the local file import log.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("ExistsLocalFileImportLog")
            .WithTags("LocalFileImportLog")
            .Produces<ExistsLocalFileImportLogRes>(StatusCodes.Status200OK)
            .Produces(400)
            .Produces(500)
            .RequireAuthorization();
        }
    }
}
