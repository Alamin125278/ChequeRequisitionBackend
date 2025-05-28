using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.GetAllBank
{
    public class GetAllBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/banks", async (int? skip, int? limit, string? search,string? isActive, ISender sender) =>
            {
                var result = await sender.Send(new GetAllBankQuery(skip ?? 0, limit ?? 10, search,isActive));
                var response = result.Adapt<GetAllBankResult>();
                return Results.Ok(response);

            });
        }
    }
}
