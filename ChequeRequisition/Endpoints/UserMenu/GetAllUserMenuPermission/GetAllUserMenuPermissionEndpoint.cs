using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetAllUserMenuPermission
{
    public class GetAllUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user-menu-permission", async (ISender sender, int? skip, int? limit, string? search, CancellationToken cancellationToken) =>
            {
                var query = new GetAllUserMenuPermissionQuery(skip??0, limit??10, search);
                var result = await sender.Send(query, cancellationToken);
                var response = result.Adapt<GetAllUserMenuPermissionResult>();
                return Results.Ok(result);
            })
              .Produces<GetAllUserMenuPermissionResult>(200)
              .WithName("GetAllUserMenuPermissions")
              .WithTags("User Menu Permissions")
              .WithSummary("Retrieves all user menu permissions with optional pagination and search parameters.")
              .WithDescription("This endpoint allows you to retrieve a list of user menu permissions, with options for pagination and searching by permission name.");
        }
    }
}
