using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllRequisition
{
    public class GetAllRequisitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/requisition", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllRequisitionQuery(skip ?? 0, limit ?? 10, search), cancellationToken);
                var response = result.Adapt<GetAllRequisitionResult>();
                return Results.Ok(response);
            }).Produces<GetAllRequisitionResult>(StatusCodes.Status200OK)
              .WithName("GetAllRequisition")
              .WithTags("Requisition");
              
        }
    }
}
