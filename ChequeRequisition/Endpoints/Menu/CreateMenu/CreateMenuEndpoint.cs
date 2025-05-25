using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.CreateMenu
{
    public class CreateMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/menu", async (CreateMenuCommand command, ISender sender, CancellationToken cancellationToken) =>
             {
                 var result = await sender.Send(command, cancellationToken);
                 return Results.Ok(result);
             })
             .WithName("CreateMenu")
             .WithTags("Menu")
             .Produces<CreateMenuResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
