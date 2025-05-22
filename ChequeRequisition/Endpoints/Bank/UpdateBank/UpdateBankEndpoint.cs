using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.UpdateBank
{
    public class UpdateBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/bank/{id}", async (int id, UpdateBankCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                if(id != command.Id)
                {
                    return Results.BadRequest("Mismatched bank ID");
                }
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<UpdateBankCommand>("application/json")
              .WithName("UpdateBank")
              .WithTags("Banks")
              .Produces<UpdateBankResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status404NotFound);
        }
    }
}
