using Carter;
using ChequeRequisiontService.Core.Dto.Vendor;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Vendor.GetAllForBank
{
    public class GetAllForBankEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/vendor/getallforbank", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetAllForBankQuery();
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(result.Vendors);
            }).WithDescription("Get all vendors for bank selection")
              .Produces<IEnumerable<VendorDto>>()
              .WithTags("Vendor");
        }
    }
}
