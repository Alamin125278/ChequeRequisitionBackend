using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Courier.GetAllCourierForDropDown
{
    public class GetAllCourierForDropDownEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/couriers/dropdown", async (ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(new GetAllCourierForDropDownQuery(), cancellationToken);
                    return Results.Ok(new ResponseDto<GetAllCourierForDropDownResult>
                    {
                        Success = true,
                        Message = "All couriers for dropdown retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while checking the couriers for dropdown",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
              .Produces<List<GetAllCourierForDropDownResult>>(StatusCodes.Status200OK)
              .WithName("GetAllCourierForDropDown")
              .WithTags("Courier")
              .RequireAuthorization()
              .WithDescription("This endpoint retrieves all couriers for dropdown from the database.");
        }
    }
}
