using ChequeRequisiontService.Core.Dto.Ftp;

namespace ChequeRequisiontService.Core.Interfaces.Services.FtpServices;

public interface IFtpService
{
    Task<IEnumerable<string>> ListFilesAsync(FtpSetting ftpSetting);
    Task<Stream> DownloadAsync(string filename,FtpSetting ftpSetting);
    Task DeleteAsync(string filename,FtpSetting ftpSetting);
}
