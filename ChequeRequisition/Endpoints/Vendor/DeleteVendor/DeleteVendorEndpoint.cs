using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Vendor.DeleteVendor
{
    public class DeleteVendorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/vendor/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteVendorCommand(id), cancellationToken);

                if (result==null)
                    return Results.NotFound($"Vendor with ID {id} not found.");

                return Results.Ok($"Vendor with ID {id} deleted successfully.");
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteVendor")
            .WithTags("Vendor")
            .WithDescription("Soft deletes a vendor by setting IsDeleted = true.");
        }
    }
}
