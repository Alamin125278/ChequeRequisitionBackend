using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChequeRequisiontService.Endpoints.Challan.GetAllChallan
{
    public class GetAllChallanEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/challans", async (
                    int? bankId,
                    int? branchId,
                    string? challanDate,
                    int? skip,
                    int? limit,
                    string? search,
                    ISender sender,
                    CancellationToken cancellationToken) =>
            {
                var query = new GetAllChallanQuery(
                    BankId: bankId,
                    BranchId: branchId,
                    ChallanDate: challanDate,
                    Skip: skip ?? 0,
                    Limit: limit ?? 10,
                    Search: search
                );

                var result = await sender.Send(query, cancellationToken);
                var response = new ResponseDto<GetAllChallanRes>
                {
                    Message = "Successfully retrieved all challans.",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
                return Results.Ok(response);
            })
                .Produces<GetAllChallanRes>(StatusCodes.Status200OK)
                .WithName("GetAllChallan")
                .WithTags("Challan")
                .RequireAuthorization()
                .WithDescription("Retrieve all challans with optional filters like bank, branch, vendor, request date, pagination, and search.");
        }
    }
}
