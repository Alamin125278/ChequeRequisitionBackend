using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.Dashboard.GetStatCard;
using ChequeRequisiontService.Models.CRDB;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Dashboard.OrderStatusTracking
{
    public class OrderStatusTrackingEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/dashboard/ordertracking", async (string timeRange, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetOrderStatusTrackingQuery(timeRange), cancellationToken);
                var response = new ResponseDto<OrderStatusTrackingRes>
                {
                    Success = true,
                    Message = "Order status tracking fetched successfully",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
                return Results.Ok(response);
            }).WithName("Get Order Status Tracking for Dashbord")
              .WithTags("Dashboard")
              .Produces<OrderStatusTrackingRes>()
              .Produces(400)
              .Produces(500)
              .RequireAuthorization();
        }
    }
}
