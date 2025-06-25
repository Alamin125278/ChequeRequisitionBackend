using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetLoggedinUser
{
    public class GetLoggedinUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user/loggedin", async (ISender sender, CancellationToken cancellationToken) =>
             {
                 var response = await sender.Send(new GetLoggedinUserQuery(), cancellationToken);
                 return Results.Ok(response);
             })
             .WithName("GetLoggedinUser")
             .RequireAuthorization()
            .Produces<GetLoggedinUserResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("User");
        }
    }
}
