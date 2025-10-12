using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.ManageSerial.UpdateEndingNumber
{
    public class UpdateEndingNumberEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/serial/end-number", async (UpdateEndingNumberCommand command, ISender sender, CancellationToken cancellation) =>
            {
                var result = await sender.Send(command, cancellation);
                var response = new ResponseDto<UpdateEndingNumberRes>
                {
                    Data = result,
                    Message = "Ending number updated successfully.",
                    Success = true,
                    StatusCode = 200
                };
                return Results.Ok(response);
            })
              .WithName("UpdateEndingNumber")
              .WithSummary("Update Ending Number for Serial")
              .Produces<ResponseDto<UpdateEndingNumberRes>>(200)
              .Produces(400)
              .Produces(500)
              .RequireAuthorization()
              .WithTags("ManageSerial")
              .WithDescription("Update the ending number for a specific bank and cheque type.");
        }
    }
}
