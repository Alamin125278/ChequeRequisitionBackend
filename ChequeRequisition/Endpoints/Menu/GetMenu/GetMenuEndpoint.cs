using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.GetMenu
{
    public class GetMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/menu/{id}", async (int id,ISender sender,CancellationToken cancellation) =>
            {
                var result = await sender.Send(new GetMenuQuery(id), cancellation);
                return result.Menu != null ?
                     Results.Ok(result) :
                     Results.NotFound(new { Message = $"Menu with ID {id} not found."
                     });
            }).WithTags("Menu")
              .WithName("GetMenuById")
              .Produces<GetMenuResponse>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .WithSummary("Get Menu by ID")
              .WithDescription("Retrieves a specific menu by its ID. Returns 404 if not found.");
        }
    }
}
