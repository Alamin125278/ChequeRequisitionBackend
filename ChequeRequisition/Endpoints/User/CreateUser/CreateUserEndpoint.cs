using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.CreateUser;

public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user", async (CreateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);

        }).Accepts<CreateUserCommand>("application/json")
          .Produces<CreateUserResult>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status400BadRequest)
          .WithName("CreateUser")
          .WithTags("User")
          .WithDescription("Create a new user.");
    }
}
