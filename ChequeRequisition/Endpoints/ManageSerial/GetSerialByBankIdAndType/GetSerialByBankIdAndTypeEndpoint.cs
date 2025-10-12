using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.ManageSerial.GetSerialByBankIdAndType;

public class GetSerialByBankIdAndTypeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/serial/end-number", async (int bankId, string chequeType, int lvs,ISender sender,CancellationToken cancellation) =>
        {
            var query = new GetSerialByBankIdAndTypeQuery(bankId, chequeType, lvs);
            var result = await sender.Send(query,cancellation);
            var response = new ResponseDto<GetSerialByBankIdAndTypeRes>
            {
                Data = result,
                Message = "Serial number retrieved successfully.",
                Success = true,
                StatusCode = 200
            };
            return Results.Ok(response);
        })
          .WithName("GetSerialByBankIdAndType")
          .WithSummary("Get Serial Number by Bank ID and Cheque Type")
          .Produces<ResponseDto<GetSerialByBankIdAndTypeRes>>(200)
          .Produces(400)
          .Produces(500);
    }
}
