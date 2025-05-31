using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.CreateBranch
{
    public class CreateBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/branch", async (CreateBranchCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<CreateBranchCommand>("application/json")
            .WithName("CreateBranch")
            .RequireAuthorization()
            .Produces<CreateBranchResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Branch");
        }
    }
}
