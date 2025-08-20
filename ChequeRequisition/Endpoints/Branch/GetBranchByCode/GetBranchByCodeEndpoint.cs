using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranchByCode;

public class GetBranchByCodeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/branch/get-by-branch-code", async (int bankId,string branchCode,string branchName,ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetBranchByCodeQuery(bankId, branchCode, branchName);
            var result = await sender.Send(query, cancellationToken);
            var response = new ResponseDto<GetBranchByCodeRes>
            {
                Message = result.HasBranch ? "Branch found." : "Branch not found.",
                Data = result,
                StatusCode =StatusCodes.Status200OK
            };
            return Results.Json(response, statusCode: response.StatusCode);
        })
          .WithName("GetBranchByCode")
          .Produces<ResponseDto<GetBranchByCodeRes>>(StatusCodes.Status200OK)
          .Produces<ResponseDto<GetBranchByCodeRes>>(StatusCodes.Status404NotFound)
          .WithTags("Branch");
    }
}
