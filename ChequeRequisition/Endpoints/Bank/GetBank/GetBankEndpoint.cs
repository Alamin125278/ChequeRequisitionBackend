using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.GetBank
{
    public class GetBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/bank/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetBankQuery(id));
                return result.Bank != null ? Results.Ok(result) : Results.NotFound();
            }).RequireAuthorization();
        }
    }
}
