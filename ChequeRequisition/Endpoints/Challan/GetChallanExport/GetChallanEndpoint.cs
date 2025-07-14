using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChequeRequisiontService.Endpoints.Challan.GetChallanExport
{
    public class GetChallanEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/challan/get-challans", async (GetChallanCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                var response = result.Adapt<GetChallanResponse>();

                return Results.Ok(response);
            }).Produces<GetChallanResponse>(StatusCodes.Status200OK)
              .WithName("GetChallan")
              .WithTags("Challan");
        }
    }
}
