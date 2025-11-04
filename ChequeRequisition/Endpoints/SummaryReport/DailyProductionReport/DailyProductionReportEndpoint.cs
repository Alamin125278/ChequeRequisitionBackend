using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.DailyProductionReport;

public class DailyProductionReportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/daily-production-report", async (string Date, ISender sender, CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new GetDailyProductionReportQuery(Date);
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(new ResponseDto<GetDailyProductionReportRes>
                {
                    Success = true,
                    Message = "Daily Production report retrieved successfully.",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return Results.Problem(
                    detail: ex.Message,
                    title: "An error occurred while retrieving the daily production report.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
          .Produces<GetDailyProductionReportRes>(StatusCodes.Status200OK)
          .WithName("GetDailyProductionReport")
          .WithTags("SummaryReport")
          .Produces(400)
          .Produces(500)
          .RequireAuthorization();
    }
}
