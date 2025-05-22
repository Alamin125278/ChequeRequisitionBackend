using Carter;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Vendor.GetVendor
{
    public class GetVendorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
          app.MapGet("/api/vendor/{id}",async(int id,ISender sender) =>
          {
              var result = await sender.Send(new GetVendorQuery(id));
              return result.Vendor != null ? Results.Ok(result) : Results.NotFound($"Vendor with ID {id} not found.");
          }).WithName("GetVendorById")
              .WithTags("Vendor")
              .Produces<GetVendorResult>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound); 
        }
    }
}
