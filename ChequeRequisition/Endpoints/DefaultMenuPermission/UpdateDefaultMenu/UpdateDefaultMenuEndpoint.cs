using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.UpdateDefaultMenu
{
    public class UpdateDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/default-menus-by-role", async (UpdateDefaultMenuCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                
                var response = await sender.Send(command , cancellationToken);
                return Results.Ok(response);
            }).WithName("UpdateDefaultMenu")
              .Produces<UpdateDefaultMenuResponse>(StatusCodes.Status200OK)
              .ProducesValidationProblem()
               .WithTags("Default Menu Permissions");
        }
    }
}
