using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Models.CRDB;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IFtpImportRepo
{
    Task<List<int>> BulkInsertRequisitionsAsync(IEnumerable<ChequeBookRequisition> list,CancellationToken cancellationToken=default);
    Task<int> InsertFtpLogAsync(FtpImportLogDto log,CancellationToken cancellationToken=default);
}
