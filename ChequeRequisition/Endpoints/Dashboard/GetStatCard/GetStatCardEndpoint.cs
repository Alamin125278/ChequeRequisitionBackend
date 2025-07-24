using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Dashboard.GetStatCard;

public class GetStatCardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/statcard", async (int? bankId,ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStatCardQuery(bankId), cancellationToken);
            var response = new ResponseDto<GetStatCardResponse>
            {
                Success = true,
                Message = "Stat Card fetched successfully",
                Data = result,
                StatusCode = StatusCodes.Status200OK
            };
            return Results.Ok(response);
        }).WithName("Get Stats Card Dashboard")
        .WithTags("Dashboard")
        .Produces<GetStatCardResponse>()
        .Produces(400)
        .Produces(500)
        .RequireAuthorization();
    }
}
