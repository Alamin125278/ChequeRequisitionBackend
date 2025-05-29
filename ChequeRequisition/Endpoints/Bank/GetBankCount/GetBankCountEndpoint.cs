using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.GetBankCount;

public class GetBankCountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bank/count", async (ISender sender,CancellationToken cancellationToken) =>
        {
            var query = new GetBankCountQuery();
            var result = await sender.Send(query,cancellationToken);
            return Results.Ok(result);
        })
          .Produces<GetBankCountResult>(200)
          .WithName("GetBankCount")
          .WithTags("Bank Endpoints")
          .WithSummary("Get the total count of banks and active banks.")
          .WithDescription("This endpoint retrieves the total number of banks and the count of active banks in the system.");
    }
}
