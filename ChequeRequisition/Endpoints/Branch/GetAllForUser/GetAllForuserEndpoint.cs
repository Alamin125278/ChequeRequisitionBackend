using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllForUser;

public class GetAllForuserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/branch/get-for-user/{bankId}", async (ISender sender, int bankId, CancellationToken cancellationToken) =>
        {
            var query = new GetAllForuserQuery(bankId);
            var response = await sender.Send(query, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetAllBranchesForUser")
        .RequireAuthorization()
        .Produces<GetAllForuserResponse>(200)
        .Produces(400)
        .WithTags("Branch");
    }
}
