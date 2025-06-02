using Carter;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using MediatR;
using System.Reflection;

namespace ChequeRequisiontService.Endpoints.User.GetAllUser
{
    public class GetAllUserEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/users", async (ISender sender, CancellationToken cancellationToken, int? skip, int? limit, string? search, string? isActive = null, int? role = null) =>
            {
                var result = await sender.Send(new GetAllUserQuery(skip ?? 0, limit ?? 10, search, isActive, role), cancellationToken);
                var response = result.Adapt<GetUserResult>();
                return Results.Ok(response);
            }).WithTags("User")
            .RequireAuthorization();
            ;
        }
    }
}
