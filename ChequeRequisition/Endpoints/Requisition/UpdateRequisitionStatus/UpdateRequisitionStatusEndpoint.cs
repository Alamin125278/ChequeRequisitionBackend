using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisitionStatus
{
    public class UpdateRequisitionStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/requisition/update-cheque-requisition", async (UpdateRequisitionStatusCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.IsUpdated ? Results.Ok(result) : Results.BadRequest("Failed to update requisition status");
            }).RequireAuthorization()
              .Produces<UpdateRequisitionStatusRes>(200)
              .Produces(400)
              .WithName("UpdateRequisitionStatus")
              .WithSummary("Updates the status of requisitions based on provided IDs and status code.");
        }
    }
}
