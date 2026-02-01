using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenusPermission.CreateUserMenuPermissions
{
    public class CreateUserMenuPermissionsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/usermenupermissions", async (CreateUserMenuPermissionsCommand command, ISender sender, CancellationToken cancellationToken) =>
             {
                 var result = await sender.Send(command, cancellationToken);
                 return Results.Ok(result);
             })
             .WithName("CreateUserMenuPermissions")
             .WithTags("UserMenuPermissions")
             .Produces<CreateUserMenuPermissionRes>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
