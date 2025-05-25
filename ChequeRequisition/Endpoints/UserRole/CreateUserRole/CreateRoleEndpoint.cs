using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.CreateUserRole;

public class CreateRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user-role", async (CreateRoleCommand command,ISender sender ,CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }).Accepts<CreateRoleCommand>("application/json")
          .Produces<CreateRoleResponse>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status400BadRequest)
          .WithName("CreateUserRole")
          .WithTags("User Roles")
          .WithDescription("Create a new user role.");
    }
}
