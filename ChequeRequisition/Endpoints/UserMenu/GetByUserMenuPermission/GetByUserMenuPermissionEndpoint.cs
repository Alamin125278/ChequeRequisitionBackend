using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetByUserMenuPermission
{
    public class GetByUserMenuPermissionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Get-menus-by-user", async ( ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetByUserMenuPermissionQuery();
                var response = await sender.Send(query, cancellationToken);
                return Results.Ok(response);
            }).RequireAuthorization();
        }
    }
}
