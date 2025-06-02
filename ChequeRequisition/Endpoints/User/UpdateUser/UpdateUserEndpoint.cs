
using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.UpdateUser
{

    public class UpdateUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // Fixed ASP0018 by correcting the route parameter syntax
            app.MapPatch("/api/user/{id}", async (int id,UpdateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched user ID");
                }
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).WithName("UpdateUser")
              .WithTags("User")
              .RequireAuthorization()
              .Produces<UpdateUserResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status404NotFound);

        }
    }
}
