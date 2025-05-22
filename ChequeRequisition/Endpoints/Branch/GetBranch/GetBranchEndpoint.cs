using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranch
{
    public class GetBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/api/branch/{id}", async(int id,ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetBranchQuery(id),cancellationToken);
                return result.Branch != null ? Results.Ok(result) : Results.NotFound();
            });
        }
    }
}
