

using ChequeRequisiontService.Core.Dto.Vendor;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IVendorRepo : IGenericRepository<VendorDto>
    {
        Task<IEnumerable<VendorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }

}
