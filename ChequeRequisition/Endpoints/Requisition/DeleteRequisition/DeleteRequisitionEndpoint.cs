using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.DeleteRequisition
{
    public class DeleteRequisitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/requisition/{id:int}", async (int id,ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteRequisitionCommand(id), cancellationToken);
               if (result == null)
                    return Results.NotFound($"Cheque Requisition with ID {id} not found.");
                return Results.Ok(result);
            }).Produces<DeleteRequisitionResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteRequisition")
            .WithTags("Cheque Requisition")
            .WithDescription("Soft deletes a Cheque Requisition by setting IsDeleted = true.");
        }
    }
}
