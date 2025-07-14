using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Challan.GetItemById
{
    public class GetItemByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/challan/items", async (
                    int id,
                    ISender sender,
                    CancellationToken cancellationToken) =>
            {
                var query = new GetItemByIdQuery(id);
                var result = await sender.Send(query, cancellationToken);
                var response = new ResponseDto<GetItemByIdResult>
                {
                    Message = "Successfully retrieved challan items.",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
                return Results.Ok(response);
            })
                .Produces<GetItemByIdResult>(StatusCodes.Status200OK)
                .WithName("GetItemById")
                .WithTags("Challan")
                .RequireAuthorization()
                .WithDescription("Retrieve all items of a specific challan by its ID.");
        }
    }
}
