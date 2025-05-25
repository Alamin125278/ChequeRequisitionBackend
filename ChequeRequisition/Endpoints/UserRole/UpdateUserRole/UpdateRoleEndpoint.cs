using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.UpdateUserRole
{
    public class UpdateRoleEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/user-role/{id}", async (int id, UpdateRoleCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                command = command with { Id = id };
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<UpdateRoleCommand>("application/json")
                .Produces<UpdateRoleResponse>(200)
                .WithName("UpdateUserRole")
                .WithTags("User Roles")
                .WithSummary("Updates an existing user role by ID.")
                .WithDescription("This endpoint allows you to update a user role's details, including its name and active status, by providing the role ID.");
        }
    }
}
