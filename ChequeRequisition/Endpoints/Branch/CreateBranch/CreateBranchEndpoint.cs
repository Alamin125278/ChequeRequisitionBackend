using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Branch.CreateBranch
{
    public class CreateBranchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/branch", async (CreateBranchCommand command, ISender sender,CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(command, cancellationToken);
                    return Results.Ok(new ResponseDto<CreateBranchResult>
                    {
                        Success = true,
                        Message = "Branch created successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while creating the branch.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }

            }).Accepts<CreateBranchCommand>("application/json")
            .WithName("CreateBranch")
            .RequireAuthorization()
            .Produces<CreateBranchResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Branch");
        }
    }
}
