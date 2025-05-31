using MediatR;
using Carter;

namespace ChequeRequisiontService.Endpoints.Branch.UpdateBranch
{
    public class UpdateBranchEndpoint: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/branch/{id}", async (int id, UpdateBranchCommand command, ISender sender) =>
            {
                var result = await sender.Send(command with { Id = id });
                return result.Branch != null ? Results.Ok(result) : Results.NotFound();
            })
            .WithName("UpdateBranch")
            .Produces<UpdateBranchResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .WithTags("Branch")
            .WithDescription("Update a branch by ID");
        }
    }
}
