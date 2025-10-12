using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using FluentValidation;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.CourierSummary;

public class CourierSummaryReportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/courier-summary-report", async (int bankId,string startDate,string endDate,int severity,bool agentType, ISender sender, CancellationToken cancellationToken) =>
       {
           try
           {
               var result = await sender.Send(new GetCourierSummaryReportQuery(bankId, startDate, endDate, severity, agentType), cancellationToken);

               return Results.Ok(new ResponseDto<GetCourierSummaryReportRes>
               {
                   Success=true,
                   Message = "Courier Summary report retrieved successfully.",
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
        .WithName("GetCourierSummaryReport")
        .WithTags("Summary Report")
        .Produces<GetCourierSummaryReportRes>()
        .Produces(400)
        .Produces(500)
        .RequireAuthorization();
    }
}
