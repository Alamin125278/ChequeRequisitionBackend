using BuildingBlocks.CQRS;
using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Auth.Login;

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command, CancellationToken.None);
            return Results.Ok(result);
        }).Accepts<LoginCommand>("application/json")
          .Produces<LoginResponse>(200)
          .ProducesProblem(400)
          .WithName("Login")
          .WithTags("Auth");
    }
}
