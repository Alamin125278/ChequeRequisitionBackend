using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using ChequeRequisiontService.Endpoints.Branch.GetAllBranch;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChequeRequisiontService.Endpoints.Challan.GetChallanExport
{
    public class GetChallanEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/challan/get-challans", async (GetChallanCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(command, cancellationToken);
                    var response = result.Adapt<GetChallanResponse>();
                    return Results.Ok(new ResponseDto<GetChallanResponse>
                    {
                        Success = true,
                        Message = "All branch retrieved successfully.",
                        Data = result,
                        StatusCode = StatusCodes.Status200OK
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        title: "An error occurred while checking all challans",
                        statusCode: StatusCodes.Status500InternalServerError);
                }

            }).Produces<GetChallanResponse>(StatusCodes.Status200OK)
              .WithName("GetChallan")
              .WithTags("Challan");
        }
    }
}
