using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.DeleteDefaultMenu
{
    public class DeleteDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/default-menu-permission/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new DeleteDefaultMenuCommand(id);
                var response = await sender.Send(command, cancellationToken);
                return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
            })
              .WithName("DeleteDefaultMenu")
              .WithTags("Default Menu Permission");
        }
    }
}
