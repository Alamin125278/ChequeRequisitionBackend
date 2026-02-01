using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.Branch.GetAllBranch;
using Mapster;
using MediatR;

namespace ChequeRequisiontService.Endpoints.UserRole.GetAllUserRole;

public class GetAllRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user-role", async (int? skip, int? limit, string? search,string? status, ISender sender, CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await sender.Send(new GetAllRoleQuery(skip ?? 0, limit ?? 10, search, status), cancellationToken);
                var response = result.Adapt<GetAllRoleResponse>();
                return Results.Ok(new ResponseDto<GetAllRoleResponse>
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
                    title: "An error occurred while checking the roles",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
          .Produces<GetAllRoleResponse>(200)
          .WithName("GetAllUserRoles")
          .WithTags("User Roles")
          .WithSummary("Retrieves all user roles with optional pagination and search parameters.")
          .WithDescription("This endpoint allows you to retrieve a list of user roles, with options for pagination and searching by role name.");
    }
}
