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
           app.MapPost("/api/challan/get-challans", async (GetChallanCommand command,ISender sender,CancellationToken ct) =>
           {
                var result = await sender.Send(command, ct);

                return Results.Ok(new ResponseDto<GetChallanResponse>
                {
                    Success = true,
                    Message = "Challan data retrieved successfully",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                });

            }).WithName("GetChallan")
               .WithTags("Challan");
        }
    }
}
