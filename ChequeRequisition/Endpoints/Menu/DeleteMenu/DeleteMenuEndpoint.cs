using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.DeleteMenu
{
    public class DeleteMenuEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/menu/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new DeleteMenuCommand(id);
                var response = await sender.Send(command, cancellationToken);
                return response.Success ? Results.Ok(response) : Results.NotFound(response);
            })
            .WithName("DeleteMenu")
            .WithTags("Menu");
        }
    }
}
