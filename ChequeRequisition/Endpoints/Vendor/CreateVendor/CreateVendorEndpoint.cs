using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Vendor.CreateVendor
{
    public class CreateVendorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/vendor", async (CreateVendorCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).Accepts<CreateVendorCommand>("application/json")
              .Produces<CreateVendorResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("CreateVendor")
              .WithTags("Vendor")
              .WithDescription("Create a new vendor.");
        }
    }
}
