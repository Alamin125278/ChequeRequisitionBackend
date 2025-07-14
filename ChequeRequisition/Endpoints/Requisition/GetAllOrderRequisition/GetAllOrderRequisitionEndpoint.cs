using Carter;
using ChequeRequisiontService.Endpoints.Requisition.GetAllRequisition;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllOrderRequisition;

public class GetAllOrderRequisitionEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/requisition/get-all-order-requisitions", async (
            int status,
    int? bankId ,
    int? branchId ,
    int? severity,
    string? requestDate,
    int? skip,
    int? limit ,
    string? search, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllOrderRequisitionQuery(status,bankId,branchId,severity,requestDate,skip ??0,limit ?? 10,search), cancellationToken);
            var response = result.Adapt<GetAllOrderRequisitionResult>();
            return Results.Ok(response);
        })
          .Produces<GetAllOrderRequisitionResult>(200)
          .WithName("GetAllOrderRequisitions")
          .RequireAuthorization()
          .WithSummary("Get all order requisitions with optional filters and pagination.");
    }
}

