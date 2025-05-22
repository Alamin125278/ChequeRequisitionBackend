using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetUser
{
    public class GetUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user/{id}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUserByIdQuery(id));
                return result.User is not null ? Results.Ok(result) : Results.NotFound();
            }).WithName("GetUserById")
              .WithTags("Users")
              .Produces<GetUserByIdResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound);
        }
    }
}
