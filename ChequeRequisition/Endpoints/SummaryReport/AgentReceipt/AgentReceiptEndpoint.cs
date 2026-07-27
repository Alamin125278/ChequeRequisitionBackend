using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.AgentReceipt
{
    public class AgentReceiptEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/challan/agent-receipt", async (int bankId, string requestDate, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new GetAgentReceiptReportQuery(bankId, requestDate);
                    var result = await sender.Send(query, cancellationToken);
                    return Results.Ok(new ResponseDto<GetAgentReceiptReportRes>
                    {
                        Success = true,
                        Message = "Agent receipt report retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while retrieving the agent receipt report.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags("Agent Receipt")
            .WithName("Get Agent Receipt")
            .WithSummary("Get Agent Receipt")
            .Produces<GetAgentReceiptReportRes>(StatusCodes.Status200OK)
            .Produces(400)
            .Produces(500);
            //.RequireAuthorization();
        }
    }
}
