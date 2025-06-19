using ChequeRequisiontService.Core.Dto.Ftp;

namespace ChequeRequisiontService.Core.Interfaces.Services.FtpServices;

public interface IFtpLogDbService
{
    Task InsertFtpLogAsync(FtpFileLogDto ftpFileLogDto,CancellationToken cancellationToken=default);
}
