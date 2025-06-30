using ChequeRequisiontService.Core.Dto.Challan;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IChallanRepo
{
    Task<int> AddChallanAsync(ChallanDto challanDto,int UserId,CancellationToken cancellation=default);
    Task<int> AddChallanRequisitionAsync(ChallanTrackingDto challanTrackingDto,int UserId,CancellationToken cancellation=default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<List<ChallanExportDto>> GetChallanExportDataAsync(List<int> challanIds, CancellationToken cancellationToken);
}
