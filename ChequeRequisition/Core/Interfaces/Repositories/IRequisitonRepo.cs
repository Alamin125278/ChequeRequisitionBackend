using ChequeRequisiontService.Core.Dto.Requisition;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IRequisitonRepo:IGenericRepository<RequisitionDto>
    {
        Task<IEnumerable<RequisitionDto>> GetAllAsync(int? Status,int? BankId, int? BranchId, int? Severity, DateOnly? RequestDate, int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<RequisitionDto>> GetAllAsync(int? Status,int? BankId, int? BranchId, int? Severity, DateOnly? RequestDate, string? Search = null, CancellationToken cancellationToken = default);
        Task<int> GetAllCountAsync(int? Status, int? BankId, int? BranchId, int? Severity, DateOnly? RequestDate, string? Search, CancellationToken cancellationToken = default);
    }
}
