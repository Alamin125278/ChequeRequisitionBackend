using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.GetAllForBranch
{
    public class GetAllForBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/api/banks/for-branch", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllForBranchQuery(), cancellationToken);
                return Results.Ok(result);
            })
            .Produces<GetAllForBranchResult>(StatusCodes.Status200OK)
            .WithName("GetAllBanksForBranch")
            .WithTags("Bank")
            .RequireAuthorization()
            .WithDescription("This endpoint retrieves all banks associated with the current branch. It does not require any parameters and returns a list of banks.");
        }
    }
}
