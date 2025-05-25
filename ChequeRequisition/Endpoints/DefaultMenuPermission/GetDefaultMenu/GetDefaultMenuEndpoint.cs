using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetDefaultMenu
{
    public class GetDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/defaultmenu/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
               var result = await sender.Send(new getDefaultMenuQuery(id), cancellationToken);
                if (result.DefaultMenuPermision == null)
                {
                    return Results.NotFound($"Default Menu Permission with ID {id} not found.");
                }
                return Results.Ok(result.DefaultMenuPermision);
            }).WithName("GetDefaultMenuById")
            .WithTags("Default Menu Permissions")
              .Produces<GetDefaultMenuResponse>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .WithSummary("Get Default Menu Permission by ID")
              .WithDescription("Retrieves a specific default menu permission by its ID. Returns 404 if not found.");
        }
    }
}
