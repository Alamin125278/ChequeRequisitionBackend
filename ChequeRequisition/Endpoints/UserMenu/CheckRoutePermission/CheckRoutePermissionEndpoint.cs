using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.CheckRoutePermission
{
    public class CheckRoutePermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/check-route-permission/{path}",
            async (string path, ISender sender, CancellationToken cancellationToken) =>
            {
                path = Uri.UnescapeDataString(path); // optional, if encoded
                var result = await sender.Send(new CheckRoutePermissionQuery(path), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("CheckRoutePermission")
            .WithSummary("Check if the user has permission for a specific route")
            .WithDescription("This endpoint checks if the authenticated user has permission to access a specific route based on their menu permissions.");
        }
    }
}
