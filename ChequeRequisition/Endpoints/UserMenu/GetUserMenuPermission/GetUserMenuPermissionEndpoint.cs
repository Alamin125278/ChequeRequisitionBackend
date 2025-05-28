using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetUserMenuPermission
{
    public class GetUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user-menu-permission/{id}", async (int id, ISender sender,CancellationToken cancellationToken) =>
            {
                var query = new GetUserMenuPermissionQuery(id);
                var response = await sender.Send(query,cancellationToken);
                return response.UserMenuPermission != null ? Results.Ok(response) : Results.NotFound($"User Menu Permission with ID {id} not found.");
            }).Produces<GetUserMenuPermissionResponse>(200)
              .WithName("GetUserMenuPermissionById")
              .WithTags("User Menu Permissions")
              .WithSummary("Retrieves a user menu permission by its ID.")
              .WithDescription("This endpoint allows you to retrieve a specific user menu permission using its unique identifier.");
        }
    }
}
