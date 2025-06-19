using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Models.CRDB;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IFtpImportRepo
{
    Task<bool> BulkInsertRequisitionsAsync(IEnumerable<ChequeBookRequisition> list,CancellationToken cancellationToken=default);
    Task<bool> InsertFtpLogAsync(FtpImportLogDto log,CancellationToken cancellationToken=default);
}
