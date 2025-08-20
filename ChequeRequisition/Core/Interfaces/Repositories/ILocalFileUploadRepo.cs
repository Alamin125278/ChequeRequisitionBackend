using ChequeRequisiontService.Core.Dto.BulkUploadResult;
using ChequeRequisiontService.Core.Dto.Requisition;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface ILocalFileUploadRepo
    {
        Task<bool> LocalFileUploadAsync(RequisitionDto requisitionDto, int userId, CancellationToken cancellationToken = default);
        Task<BulkUploadResultDto> BulkUploadAsync(List<RequisitionDto> Items, int UserId,int? VendorId, CancellationToken cancellationToken);

    }
}
