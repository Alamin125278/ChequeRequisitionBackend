using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllBranch
{
    public class GetAllBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/branches", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllBranchQuery(skip ?? 0, limit ?? 10, search), cancellationToken);
                var response = result.Adapt<GetAllBranchResult>();
                return Results.Ok(response);
            })
              .Produces<GetAllBranchResult>(StatusCodes.Status200OK)
              .WithName("GetAllBranch")
              .WithTags("Branch")
              .WithDescription("This endpoint retrieves all branches from the database. You can specify pagination parameters like skip and limit, as well as a search term to filter the results.");
        }
    }
}
