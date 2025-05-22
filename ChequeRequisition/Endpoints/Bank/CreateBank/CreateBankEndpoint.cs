using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.CreateBank
{
    public class CreateBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/bank", async (CreateBankCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<CreateBankCommand>("application/json")
            .WithName("CreateBank")
            .Produces<CreateBankResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Bank");
        }
    }
}
