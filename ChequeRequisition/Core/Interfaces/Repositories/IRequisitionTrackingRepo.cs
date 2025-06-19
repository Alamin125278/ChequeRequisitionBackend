using ChequeRequisiontService.Core.Dto.Ftp;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IRequisitionTrackingRepo
{
    Task InsertTrackingAsync(List<RequisitionImportTrackingDto> trackings, CancellationToken cancellationToken = default);
}
