using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.DefaultMenusByRole
{
    public class DefaultMenusByRoleEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/default-menus-by-role/{roleId}", async (int roleId, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(new GetAllDefaultMenusByRoleQuery(roleId), cancellationToken);
                    var response = result.Adapt<GetAllDefaultMenusByRoleRes>();
                    return Results.Ok(new ResponseDto<GetAllDefaultMenusByRoleRes> 
                    { 
                        Success = true,
                        Message= "Default Menus by role retrieved successfully.",
                        Data = response,
                        StatusCode = StatusCodes.Status200OK
                    });

                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while checking the Menus",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithName("GetDefaultMenusByRole")
              .WithTags("Default Menu Permissions");
        }
    }
}
