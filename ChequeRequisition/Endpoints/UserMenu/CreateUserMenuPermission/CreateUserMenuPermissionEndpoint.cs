using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.CreateUserMenuPermission
{
    public class CreateUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapPost("/api/user-menu-permission", async (CreateUserMenuPermissionCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<CreateUserMenuPermissionCommand>("application/json")
            .WithName("CreateUserMenuPermission")
            .WithTags("User Menu Permissions")
            .Produces<CreateUserMenuPermissionResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
