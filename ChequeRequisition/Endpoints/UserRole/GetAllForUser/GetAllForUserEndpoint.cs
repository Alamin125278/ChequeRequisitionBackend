using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.GetAllForUser
{
    public class GetAllForUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/userrole/getallforuser", async (ISender sender, CancellationToken cancellationToken) =>
             {
                 var query = new GetAllForUserQuery();
                 var response = await sender.Send(query, cancellationToken);
                 return Results.Ok(response);
             })
             .WithName("GetAllUserRolesForUser")
             .RequireAuthorization()
            .Produces<GetAllForUserResponse>(200)
            .Produces(400)
            .WithTags("UserRole");
        }
    }
}
