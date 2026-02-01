using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport.CourierSummary;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserMenusPermission.GetUserMenusPermissions
{
    public class GetUserMenusPermissionsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user-menus-permissions", async (int userId, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    //var query = new GetUserMenusPermissionsQuery(userId);
                    var result = await sender.Send(new GetUserMenusPermissionsQuery(userId), cancellationToken);
                    return Results.Ok(new ResponseDto<GetUserMenusPermissionsRes>
                    {
                        Success = true,
                        Message = "User Menus Permissions retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while retrieving the user menus permissions.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
              .WithName("GetUserMenusPermissions")
                .WithTags("User Menus Permissions")
                .Produces<GetUserMenusPermissionsRes>()
                .Produces(400)
                .Produces(500)
                .RequireAuthorization();
        }
    }
}
