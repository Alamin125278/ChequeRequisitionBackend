using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllBranch
{
    public class GetAllBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/branches", async (int? skip, int? limit,int? bankId, string? search, string ? isActive, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(new GetAllBranchQuery(skip ?? 0, limit ?? 10, bankId, search, isActive), cancellationToken);
                    var response = result.Adapt<GetAllBranchResult>();
                    return Results.Ok(new ResponseDto<GetAllBranchResult>
                    {
                        Success = true,
                        Message = "All branch retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while checking the branches",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
              .Produces<GetAllBranchResult>(StatusCodes.Status200OK)
              .WithName("GetAllBranch")
              .WithTags("Branch")
              .RequireAuthorization()
              .WithDescription("This endpoint retrieves all branches from the database. You can specify pagination parameters like skip and limit, as well as a search term to filter the results.");
        }
    }
}
