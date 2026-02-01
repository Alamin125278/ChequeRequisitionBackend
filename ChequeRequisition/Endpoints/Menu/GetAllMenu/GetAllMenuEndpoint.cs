using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.Branch.GetAllBranch;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Menu.GetAllMenu
{
    public class GetAllMenuEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/menu", async (int? skip, int? limit, string? search, ISender sender, CancellationToken cancellation) =>
            {
                try
                {
                    var result = await sender.Send(new GetAllMenuQuery(skip ?? 0, limit ?? 10, search), cancellation);
                    return Results.Ok(new ResponseDto<GetAllMenuResponse>
                    {
                        Success = true,
                        Message = "All menu retrieved successfully.",
                        Data = result,
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
            }).WithName("GetAllMenu")
                .WithTags("Menu");
        }
    }
}
