using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.UpdateUserMenuPermission
{
    public class UpdateUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/user-menu-permission/{id}", async (int id, UpdateUserMenuPermissionCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched User Menu Permission ID");
                }
                var response = await sender.Send(command with { Id = id }, cancellationToken);
                return Results.Ok(response);
            }).Accepts<UpdateUserMenuPermissionCommand>("application/json")
                .Produces<UpdateUserMenuPermissionResult>(200)
                .WithName("UpdateUserMenuPermission")
                .WithTags("User Menu Permissions")
                .WithSummary("Updates an existing user menu permission by ID.")
                .WithDescription("This endpoint allows you to update a user menu permission's details, including its user ID, menu ID, and active status, by providing the permission ID.");
        }
    }
}
