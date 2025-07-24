using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Dashboard.BankWiserRequisition;

public class BankWiserRequisitionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/bankwiserequisition", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBankWiserRequisitionQuery(), cancellationToken);
            var response = new ResponseDto<BankWiserRequisitionResponse>
            {
                Success = true,
                Message = "Bank Wise Requisition fetched successfully",
                Data = result,
                StatusCode = StatusCodes.Status200OK
            };
            return Results.Ok(response);
        })
          .WithName("Get Bank Wise Requisition Dashboard")
          .WithTags("Dashboard")
          .Produces<BankWiserRequisitionResponse>()
          .Produces(400)
          .Produces(500)
          .RequireAuthorization();
    }
}
