using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using FluentValidation;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport;

public class SummaryReportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/summary-report", async (int bankId,string startDate,string endDate,int severity, ISender sender, CancellationToken cancellationToken) =>
       {
           try
           {
               var result = await sender.Send(new GetSummaryReportQuery(bankId, startDate, endDate, severity), cancellationToken);

               return Results.Ok(new ResponseDto<GetSummaryReportRes>
               {
                   Success=true,
                   Message = "Summary report retrieved successfully.",
                   Data = result,
                   StatusCode = StatusCodes.Status200OK
               });
           }
           catch (Exception ex)
           {
               // Log the exception here if needed
               return Results.Problem(
                   detail: ex.Message,
                   title: "An error occurred while retrieving the summary report.",
                   statusCode: StatusCodes.Status500InternalServerError);
           }
       })
        .WithName("GetSummaryReport")
        .WithTags("Summary Report")
        .Produces<GetSummaryReportRes>()
        .Produces(400)
        .Produces(500)
        .RequireAuthorization();
    }
}
