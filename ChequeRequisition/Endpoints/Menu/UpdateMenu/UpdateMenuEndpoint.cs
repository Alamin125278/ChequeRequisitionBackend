using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.UpdateMenu
{
    public class UpdateMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapPut("/api/menu/{id}", async (int id, UpdateMenuCommand command,ISender sender,CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched menu ID");
                }
                var response = await sender.Send(command with { Id = id }, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("UpdateMenu")
            .Produces<UpdateMenuResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .WithTags("Menu");
        }
    }
}
