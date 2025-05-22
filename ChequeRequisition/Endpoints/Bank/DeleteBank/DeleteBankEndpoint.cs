using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Bank.DeleteBank
{
    public class DeleteBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/bank/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteBankCommand(id), cancellationToken);
                if (result == null)
                    return Results.NotFound($"Bank with ID {id} not found.");
                return Results.Ok($"Bank with ID {id} deleted successfully.");
            })
            .Produces<DeleteBankResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteBank")
            .WithTags("Bank")
            .WithDescription("Soft deletes a bank by setting IsDeleted = true.");
        }
    }
}
