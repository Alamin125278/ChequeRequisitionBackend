using BuildingBlocks.CQRS;
using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Auth.ChangedPass;

public class ChangedPassEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapPost("/api/auth/change-password", async (ChangePasswordCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                  ? Results.Ok(result)
                  : Results.BadRequest(new { message = result.Message });
        })
        .WithName("ChangePassword")
        .WithTags("Auth")
        .Accepts<ChangePasswordCommand>("application/json")
        .RequireAuthorization()
        .Produces<ChangePasswordResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
