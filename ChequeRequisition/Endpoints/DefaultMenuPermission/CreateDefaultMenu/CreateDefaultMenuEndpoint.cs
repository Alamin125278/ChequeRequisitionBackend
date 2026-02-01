using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.CreateDefaultMenu
{
    public class CreateDefaultMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/default-menus-by-role", async (CreateDefaultMenuCommand command, ISender sender, CancellationToken cancellationToken) =>
             {
                 try
                 {
                     var response = await sender.Send(command, cancellationToken);
                     return Results.Ok(new ResponseDto<CreateDefaultMenuResponse>
                     {
                         Success = true,
                         Message = "Created Default Role Menus Successfully.",
                         Data = response,
                         StatusCode = StatusCodes.Status200OK
                     });
                 }
                 catch (Exception ex)
                 {
                     // Log the exception here if needed
                     return Results.Problem(
                         detail: ex.Message,
                         title: "An error occurred while checking the Default Role Menus",
                         statusCode: StatusCodes.Status500InternalServerError);
                 }
             })
             .WithName("CreateDefaultMenu")
             .WithTags("Default Menu Permission")
             .Produces<CreateDefaultMenuResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
