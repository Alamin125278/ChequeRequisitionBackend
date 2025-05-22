using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Vendor.GetAllVendor
{
    public record GetAllVendorQuery(int Skip = 0, int Limit = 10, string? Search = null) :IQuery<GetAllVendorResult>;
   public record GetAllVendorResult(string Message, IEnumerable<Core.Dto.Vendor.VendorDto> VendorDtos);
    public class GetAllVendorHandler(IVendorRepo vendorRepo): IQueryHandler<GetAllVendorQuery, GetAllVendorResult>
    {
        private readonly IVendorRepo _vendorRepo = vendorRepo;
        public async Task<GetAllVendorResult> Handle(GetAllVendorQuery request, CancellationToken cancellationToken)
        {
            var vendors = await _vendorRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
            return new GetAllVendorResult("Retreiving Vendors Success.", vendors);
        }
    }
}
