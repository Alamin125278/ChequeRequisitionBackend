using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Vendor.UpdateVendor
{
    public class UpdateVendorEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // Fixed ASP0018 by correcting the route parameter syntax
            app.MapPatch("/api/vendor/{id}", async (int id, UpdateVendorCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Mismatched vendor ID");
                }
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            }).WithName("UpdateVendor")
              .WithTags("Vendors")
              .Produces<UpdateVendorResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status404NotFound);
        }
    }
}
