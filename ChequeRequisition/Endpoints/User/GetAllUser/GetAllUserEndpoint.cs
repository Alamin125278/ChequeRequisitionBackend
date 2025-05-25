using Carter;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetAllUser
{
    public class GetAllUserEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/users", async (int? skip, int? limit, string? search, ISender sender) =>
            {
                var result = await sender.Send(new GetAllUserQuery(skip ?? 0, limit ?? 10, search));
                var response = result.Adapt<GetUserResult>();
                return Results.Ok(response);
            }).WithTags("User");
        }
    }
}
