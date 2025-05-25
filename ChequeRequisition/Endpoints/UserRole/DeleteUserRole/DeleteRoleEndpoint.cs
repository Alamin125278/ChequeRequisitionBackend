using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.DeleteUserRole
{
    public class DeleteRoleEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/user-role/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
             {
                 var command = new DeleteRoleCommand(id);
                 var result = await sender.Send(command, cancellationToken);
                 if (result == null)
                     return Results.NotFound($"User Role with ID {id} not found.");
                 return Results.Ok($"User Role with ID {id} deleted successfully.");
             })
             .WithName("DeleteUserRole")
            .WithTags("User Roles")
             .WithDescription("Soft deletes a user role by setting IsDeleted = true.");
        }
    }
}
