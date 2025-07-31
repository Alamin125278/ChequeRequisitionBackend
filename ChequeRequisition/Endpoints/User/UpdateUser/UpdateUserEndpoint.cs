
using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.UpdateUser
{

    public class UpdateUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // Fixed ASP0018 by correcting the route parameter syntax
            app.MapPatch("/api/user/{id}", async (int id,UpdateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched user ID");
                }
                var result = await sender.Send(command, cancellationToken);
                var response = new ResponseDto<UpdateUserResult>
                {
                    Success =true,
                    Message = "Update User Successfully",
                    StatusCode = 202,
                    Data=result
                };
                return Results.Ok(response);
            }).WithName("UpdateUser")
              .WithTags("User")
              .RequireAuthorization()
              .Produces<UpdateUserResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status404NotFound);

        }
    }
}
