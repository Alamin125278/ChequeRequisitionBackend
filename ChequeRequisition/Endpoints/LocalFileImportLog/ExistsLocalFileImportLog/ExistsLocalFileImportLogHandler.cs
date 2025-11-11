using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.LocalFileImportLog.ExistsLocalFileImportLog;
public record ExistsLocalFileImportLogQuery(int BankId, string Filename):IQuery<ExistsLocalFileImportLogRes>;

public record ExistsLocalFileImportLogRes(bool Exists);

public class ExistsLocalFileImportLogHandler(ILocalFileImportLogRepo localFileImportLogRepo) : IQueryHandler<ExistsLocalFileImportLogQuery, ExistsLocalFileImportLogRes>
{
    public async Task<ExistsLocalFileImportLogRes> Handle(ExistsLocalFileImportLogQuery request, CancellationToken cancellationToken)
    {
        var exists = await localFileImportLogRepo.ExistsAsync(request.BankId, request.Filename, cancellationToken);
        return new ExistsLocalFileImportLogRes(exists);
    }

}
