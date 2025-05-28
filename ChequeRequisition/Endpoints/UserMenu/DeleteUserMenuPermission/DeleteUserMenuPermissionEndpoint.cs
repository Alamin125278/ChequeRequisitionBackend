using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.DeleteUserMenuPermission
{
    public class DeleteUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/user-menu-permission/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new DeleteUserMenuPermissionCommand(id);
                var response = await sender.Send(command, cancellationToken);
                if (response == null) {
                    return Results.NotFound($"User Menu Permission with ID {id} not found.");
                }
                return Results.Ok($"User Menu Permission with ID {id} deleted successfully.");
            })
              .Produces<DeleteUserMenuPermissionResponse>(200)
              .ProducesProblem(400, "application/problem+json")
              .WithName("DeleteUserMenuPermission")
              .WithSummary("Deletes a user menu permission by ID.")
              .WithTags("User Menu Permissions");


        }
    }
}
