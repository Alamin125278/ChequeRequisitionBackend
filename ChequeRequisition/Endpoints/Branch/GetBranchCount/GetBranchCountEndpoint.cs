using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranchCount;

public class GetBranchCountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/branch/count", async (ISender sender,CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBranchCountQuery(),cancellationToken);
            return Results.Ok(result);
        }).Produces<GetBranchCountResult>(200)
          .WithName("GetBranchCount")
          .WithTags("Branch Endpoints")
          .RequireAuthorization()
          .WithSummary("Get the total count of branches and active branches.")
          .WithDescription("This endpoint retrieves the total number of branches and the count of active branches in the system.");
    }
}
