using ChequeRequisiontService.Core.Dto.Dashboard;

namespace ChequeRequisiontService.Core.Interfaces.Repositories.Dashboard;

public interface IDashboardRepo
{
    Task<int> GetAllCountAsync(int? Status,int? BankId,int? BranchId,int? VendorId,CancellationToken cancellationToken=default);
    Task<IEnumerable<BankWiserRequisitonDto>> GetBankWiseRequisitionAsync(int? VendorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderTrackingDto>> GetOrderTrackingAsync(DateOnly StartDate,DateOnly EndDate,int? BankId,int? BranchId,int? VendorId, CancellationToken cancellationToken = default);
}
