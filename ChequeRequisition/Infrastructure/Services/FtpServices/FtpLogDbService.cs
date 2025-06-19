using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

public class FtpLogDbService(CRDBContext cRDBContext) : IFtpLogDbService
{
    private readonly CRDBContext _cRDBContext = cRDBContext;

    public async Task InsertFtpLogAsync(FtpFileLogDto ftpFileLogDto, CancellationToken cancellationToken = default)
    {
        var entity = ftpFileLogDto.Adapt<FtpFileShareLog>();
        entity.CreatedAt = DateTime.UtcNow;
        await _cRDBContext.FtpFileShareLogs.AddAsync(entity);
        await _cRDBContext.SaveChangesAsync(cancellationToken);
    }
}
