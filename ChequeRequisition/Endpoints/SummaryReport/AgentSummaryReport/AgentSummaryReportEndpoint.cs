using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport.AgentReceipt;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.AgentSummaryReport
{
    public class AgentSummaryReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/challan/agent-summary-report", async (int bankId, string requestDate,bool? agentType,string? courier, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new GetAgentSummaryReportQuery(bankId, requestDate, agentType, courier);
                    var result = await sender.Send(query, cancellationToken);
                    return Results.Ok(new ResponseDto<GetAgentSummaryReportRes>
                    {
                        Success = true,
                        Message = "Agent summary report retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while retrieving the agent summary report.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithName("Get Agent Summary Report")
            .WithSummary("Get Agent Summary Report")
            .Produces<GetAgentSummaryReportRes>(StatusCodes.Status200OK)
            .Produces(400)
            .Produces(500);
        }
    }
}
