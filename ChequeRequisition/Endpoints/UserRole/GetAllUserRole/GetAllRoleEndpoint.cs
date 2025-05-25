using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.GetAllUserRole;

public class GetAllRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user-role", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllRoleQuery(skip ?? 0, limit ?? 10, search), cancellationToken);
            var response = result.Adapt<GetAllRoleResponse>();
            return Results.Ok(response);
        })
          .Produces<GetAllRoleResponse>(200)
          .WithName("GetAllUserRoles")
          .WithTags("User Roles")
          .WithSummary("Retrieves all user roles with optional pagination and search parameters.")
          .WithDescription("This endpoint allows you to retrieve a list of user roles, with options for pagination and searching by role name.");
    }
}
