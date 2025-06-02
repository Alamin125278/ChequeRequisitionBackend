using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetUserCount
{
    public class GetUserCountEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user/count", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetUserCountQuery(), cancellationToken);
                return Results.Ok(result);
            })
              .Produces<GetUserCountResult>(200)
              .WithName("GetUserCount")
              .WithTags("User Endpoints")
              .RequireAuthorization()
              .WithSummary("Get the total count of users and active users.")
              .WithDescription("This endpoint retrieves the total number of users and the count of active users in the system.");
        }
    }
}
