using Carter;
using Mapster;
using MediatR;
using System.Collections.Generic;

namespace ChequeRequisiontService.Endpoints.Vendor.GetAllVendor
{
    public class GetAllVendorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/vendors", async (int? skip, int? limit, string? search, ISender sender) =>
            {
                var result = await sender.Send(new GetAllVendorQuery(skip ?? 0,limit ??10,search));
                var response = result.Adapt<GetAllVendorResult>();
                return Results.Ok(response);
            });
        }
    }
}
