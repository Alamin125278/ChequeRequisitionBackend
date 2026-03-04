using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisitionSeverity
{
    public class UpdateRequisitionSeverityEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/requisition/update-cheque-severity", async (UpdateRequisitionSeverityCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.IsUpdated ? Results.Ok(result) : Results.BadRequest("Failed to update requisition severity");
            }).RequireAuthorization()
              .Produces<UpdateRequisitionSeverityRes>(200)
              .Produces(400)
              .WithName("UpdateRequisitionSeverity")
              .WithSummary("Updates the severity of requisitions based on provided IDs and severity level.");
        }
    }
}
