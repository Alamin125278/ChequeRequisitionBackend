using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetAllDefaultMenu
{
    public class GetAllDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/default-menu-permissions", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellationToken) =>
            {
                var result=await sender.Send(new GetAllDefautlMenuQuery(skip ?? 0, limit ?? 10, search), cancellationToken);
                return Results.Ok(result.Adapt<GetAllDefautlMenuQueryResponse>());
            }).WithName("GetAllDefaultMenu")
              .WithTags("Default Menu Permissions");
        }
    }
}
