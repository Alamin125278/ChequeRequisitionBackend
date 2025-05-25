using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.GetUserRole;

public class GetRoleEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user-role/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRoleQuery(id), cancellationToken);
            return result.UserRole != null ?
                         Results.Ok(result) :
                         Results.NotFound(new
                         {
                             Message = $"User Role with ID {id} not found."
                         });
        })
          .Produces<GetRoleResponse>(200)
          .WithName("GetUserRoleById")
          .WithTags("User Roles")
          .WithSummary("Retrieves a user role by its ID.")
          .WithDescription("This endpoint allows you to retrieve a specific user role using its unique identifier.");
    }
}
