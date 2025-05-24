using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.GetRequisition
{
    public class GetRequisitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/api/requisition/{id}", async(int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetRequisitionQuery(id), cancellationToken);
                return result.RequisitionDto != null ? Results.Ok(result) : Results.NotFound();
            }).Produces<GetRequisitionResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .WithName("GetRequisition")
              .WithTags("Requisition");
        }
    }
}
