using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.CreateDefaultMenu
{
    public class CreateDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/default-menu-permission", async (CreateDefaultMenuCommand command, ISender sender, CancellationToken cancellationToken) =>
             {
                 var response = await sender.Send(command, cancellationToken);
                 return Results.Ok(response);
             })
             .WithName("CreateDefaultMenu")
             .WithTags("Default Menu Permission")
             .Produces<CreateDefaultMenuResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
