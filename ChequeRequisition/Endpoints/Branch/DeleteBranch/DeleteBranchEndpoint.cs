using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.DeleteBranch
{
    public class DeleteBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/branch/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteBranchCommand(id), cancellationToken);
                if (result == null)
                    return Results.NotFound($"Branch with ID {id} not found.");
                return Results.Ok($"Branch with ID {id} deleted successfully.");
            }).Produces<DeleteBranchResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteBranch")
            .WithTags("Branch")
            .WithDescription("Soft deletes a Branch by setting IsDeleted = true.");
        }
    }
}
