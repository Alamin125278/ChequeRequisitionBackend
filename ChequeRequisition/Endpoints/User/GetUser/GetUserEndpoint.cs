using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetUser
{
    public class GetUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetUserByIdQuery(id));
                return result.User is not null ? Results.Ok(result) : Results.NotFound();
            }).WithName("GetUserById")
              .WithTags("User")
              .Produces<GetUserByIdResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound);
        }
    }
}
