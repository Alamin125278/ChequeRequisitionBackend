using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisition
{
    public class UpdateRequisitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/requisition/{id}", async (int id, UpdateRequisitionCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command with { Id = id }, cancellationToken);
                return result.Requisition != null ? Results.Ok(result) : Results.NotFound();
            }).Produces<UpdateRequisitionResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("UpdateRequisition")
              .WithTags("Requisition")
              .WithDescription("Update a requisition by ID");
        }
    }
}
