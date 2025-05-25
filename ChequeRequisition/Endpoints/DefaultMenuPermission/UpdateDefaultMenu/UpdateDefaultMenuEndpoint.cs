using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.UpdateDefaultMenu
{
    public class UpdateDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/default-menu-permission/{id}", async (int id ,UpdateDefaultMenuCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched DefaultMenuPermission ID");
                }
                var response = await sender.Send(command with { Id = id }, cancellationToken);
                return Results.Ok(response);
            }).WithName("UpdateDefaultMenu")
              .Produces<UpdateDefaultMenuResponse>(StatusCodes.Status200OK)
              .ProducesValidationProblem()
               .WithTags("Default Menu Permissions");
        }
    }
}
