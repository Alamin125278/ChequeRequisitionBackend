using ChequeRequisiontService.Core.Dto.Challan;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IChallanRepo
{
    Task<int> AddChallanAsync(ChallanDto challanDto,int UserId,CancellationToken cancellation=default);
    Task<int> AddChallanRequisitionAsync(ChallanTrackingDto challanTrackingDto,int UserId,CancellationToken cancellation=default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<List<ChallanExportDto>> GetChallanExportDataAsync(List<int> challanIds, CancellationToken cancellationToken);
    Task<IEnumerable<ChallanDto>> GetAllAsync(int? BankId, int? BranchId, int? VendorId, DateOnly? RequestDate, int Skip = 0, int Limit = 10, string? Search=null, CancellationToken cancellationToken = default);
    Task<int> GetAllCountAsync(int? BankId, int? BranchId, int? VendorId, DateOnly? RequestDate,string? Search=null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChallanItemDto>>GetAllItemAsync(int Id, CancellationToken cancellationToken= default);
    Task<int>GetAllItemCountAsync(int Id, CancellationToken cancellationToken= default);
    Task<int> GetChallanNumber(int BankId, CancellationToken cancellationToken = default);
}
