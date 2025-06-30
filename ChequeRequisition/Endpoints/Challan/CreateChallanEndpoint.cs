using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Challan
{
    public class CreateChallanEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/challan/create", async (
     CreateChallanCommand command,
     ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).WithName("CreateChallan")
            .RequireAuthorization()
            .Produces<CreateChallanRes>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Challan");
        }
    }
}
