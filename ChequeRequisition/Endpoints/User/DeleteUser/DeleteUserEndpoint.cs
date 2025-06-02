using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.DeleteUser
{
    public class DeleteUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/user/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteUserCommand(id), cancellationToken);
                if (result == null)
                    return Results.NotFound($"User with ID {id} not found.");
                return Results.Ok($"User with ID {id} deleted successfully.");
            }).Produces(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .WithName("DeleteUser")
              .WithTags("User")
              .RequireAuthorization()
              .WithDescription("Soft deletes a user by setting IsDeleted = true.");
        }
    }
}
