using ChequeRequisiontService.Core.Dto.Bank;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IBankRepo:IGenericRepository<BankDto>
{
    Task<IEnumerable<BankDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null,bool? IsActive=null,CancellationToken cancellationToken=default);
    Task<int> GetAllCountAsync(string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankDto>> GetAllAsync(int? BankId, int? VendorId = null, CancellationToken cancellationToken=default);
}
