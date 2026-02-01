using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.User.GetUsers
{
    public class GetUsersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/all-users", async (ISender sender,CancellationToken cancellationToken) =>
            {
                try
                {
                var result = await sender.Send(new GetUsersQuery(), cancellationToken);
                    return Results.Ok(new ResponseDto<GetUsersRes> 
                    { 
                        Success = true,
                        Message = "Users retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });

                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while retrieving users.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags("User")
            .RequireAuthorization();
        }
    }
}
