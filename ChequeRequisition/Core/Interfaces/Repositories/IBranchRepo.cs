
using ChequeRequisiontService.Core.Dto.Branch;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IBranchRepo:IGenericRepository<BranchDto>
{
    Task<IEnumerable<BranchDto>> GetAllAsync(int? BankId=null, int Skip = 0, int Limit = 10, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<BranchDto>> GetAllAsync(int? BankId = null, CancellationToken cancellationToken = default);
    Task<int> GetAllCountAsync(string? Search = null, int? BankId =null ,bool? IsActive = null, CancellationToken cancellationToken = default);
    Task<int>GetIdAsync(int BankId,string BranchName, CancellationToken cancellationToken = default);
}
