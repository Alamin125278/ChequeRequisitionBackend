using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.GetAllMenu
{
    public class GetAllMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/menu", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellation) =>
            {
                var result = await sender.Send(new GetAllMenuQuery(skip ?? 0, limit ?? 10, search), cancellation);
                return Results.Ok(result.Adapt<GetAllMenuResponse>());
            }).WithName("GetAllMenu")
                .WithTags("Menu");
        }
    }
}
