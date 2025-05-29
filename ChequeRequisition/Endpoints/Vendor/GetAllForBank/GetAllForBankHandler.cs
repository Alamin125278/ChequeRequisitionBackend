using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Vendor.GetAllForBank;

public record GetAllForBankQuery():IQuery<GetAllForBankResult>;
public record GetAllForBankResult(IEnumerable<VendorDto> Vendors);
public class GetAllForBankHandler(IVendorRepo vendorRepo) : IQueryHandler<GetAllForBankQuery, GetAllForBankResult>
{
    private readonly IVendorRepo _vendorRepo = vendorRepo;
    public async Task<GetAllForBankResult> Handle(GetAllForBankQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _vendorRepo.GetAllAsync(cancellationToken);
        return new GetAllForBankResult(vendors);
    }
}
