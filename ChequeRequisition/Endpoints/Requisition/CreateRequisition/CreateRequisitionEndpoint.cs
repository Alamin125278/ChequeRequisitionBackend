using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.CreateRequisition
{
    public class CreateRequisitionEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/requisition", async (CreateRequisitionCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<CreateRequisitionCommand>("application/json")
            .WithName("CreateRequisition")
            .Produces<CreateRequisitionResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Requisition");
        }
    }
}
