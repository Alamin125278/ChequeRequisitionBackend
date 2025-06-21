using ChequeRequisiontService.Core.Dto.Requisition;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface ILocalFileUploadRepo
    {
        Task<bool> LocalFileUploadAsync(RequisitionDto requisitionDto, int userId, CancellationToken cancellationToken = default);
    }
}
