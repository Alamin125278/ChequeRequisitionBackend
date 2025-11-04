using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport
{
    public class ConsumptionReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/api/consumption-report", async (int bankId, string startDate,string endDate,ISender sender,CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new GetConsumptionReportQuery(bankId,startDate, endDate);
                    var result = await sender.Send(query, cancellationToken);
                    return Results.Ok(new ResponseDto<GetConsumptionReportRes>
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
                        title: "An error occurred while retrieving the consumption report.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithTags("Summary Report")
            .WithName("Get Consumption Report")
            .WithSummary("Get Consumption Report")
            .Produces<GetConsumptionReportRes>(StatusCodes.Status200OK)
            .Produces(400)
            .Produces(500)
            .RequireAuthorization();
        }
    }
}
