using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetRoleByDefaultMenu
{
    public class GetRoleByDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/menu/get-menu-id-by-role-and-menu-id/{roleId:int}/{menuId:int}", async (int roleId, int menuId, ISender sender, CancellationToken cancellationToken) =>
            {
                try { 
                    var query = new GetRoleByDefaultMenuQuery(roleId, menuId);
                    var response = await sender.Send(query, cancellationToken);
                    return Results.Ok(new ResponseDto<GetRoleByDefaultMenuResponse> 
                    { 
                        Success = true,
                        Message = "Retrieved Default Role Menus Successfully.",
                        Data = response,
                        StatusCode = StatusCodes.Status200OK
                    });

                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while retrieving the Default Role Menus",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
              .WithName("GetRoleByDefaultMenu")
              .WithTags("DefaultMenuPermission");
        }
    }
}
