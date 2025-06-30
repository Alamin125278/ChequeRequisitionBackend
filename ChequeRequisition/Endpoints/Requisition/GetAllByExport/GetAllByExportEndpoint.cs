using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllByExport;

public class GetAllByExportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
      app.MapGet("/api/requisition/get-all-order-requisitions-for-export", async (
            int? bankId,
            int? branchId,
            int? severity,
            string? requestDate,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllByExportQuery(bankId, branchId, severity, requestDate, search), cancellationToken);
            var response = result.Adapt<GetAllByExportResult>();
            return Results.Ok(response);
        })
        .Produces<GetAllByExportResult>(200)
        .WithName("GetAllByExport")
        .WithSummary("Get all requisitions by export with optional filters.");
    }
}
