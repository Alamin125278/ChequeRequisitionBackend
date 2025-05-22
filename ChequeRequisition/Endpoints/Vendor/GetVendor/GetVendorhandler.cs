using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Vendor.GetVendor
{
    public record GetVendorQuery(int Id) : IQuery<GetVendorResult>;
    public record GetVendorResult(VendorDto? Vendor);
    public class GetVendorhandler(IVendorRepo vendorRepo):IQueryHandler<GetVendorQuery, GetVendorResult>
    {
        public async Task<GetVendorResult> Handle(GetVendorQuery request, CancellationToken cancellationToken)
        {
            var vendor = await vendorRepo.GetByIdAsync(request.Id, cancellationToken);
            return vendor == null ? throw new NotFoundException($"Vendor with ID {request.Id} not found.") : new GetVendorResult(vendor);
        }
    }
}
