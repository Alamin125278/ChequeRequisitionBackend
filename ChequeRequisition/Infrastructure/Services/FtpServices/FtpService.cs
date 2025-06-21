using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Services;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

//public class FtpService(IOptions<List<FtpSetting>> ftpOptions) : IFtpService
//{
//    private readonly FtpSetting _ftp = ftpOptions.Value.First();

//    public async Task<IEnumerable<string>> ListFilesAsync()
//    {
//        using var client = new FtpClient(_ftp.Host, _ftp.User, _ftp.Password);
//        client.EncryptionMode = FtpEncryptionMode.Explicit;
//        client.ValidateAnyCertificate = true;
//        await client.ConnectAsync();
//        var fullPaths = await client.GetNameListingAsync(_ftp.RemotePath);
//        return fullPaths
//      .Select(p => Path.GetFileName(p))
//      .Where(name => name != null) 
//      .Cast<string>(); 
//    }

//    public async Task<Stream> DownloadAsync(string filename)
//    {
//        var stream = new MemoryStream();
//        using var client = new FtpClient(_ftp.Host, _ftp.User, _ftp.Password);
//        client.EncryptionMode = FtpEncryptionMode.Explicit;
//        client.ValidateAnyCertificate = true;
//        await client.ConnectAsync();
//        await client.DownloadAsync(stream, $"{_ftp.RemotePath}/{filename}");
//        stream.Position = 0; 
//        return stream;
//    }

//    public async Task DeleteAsync(string filename)
//    {
//        using var client = new FtpClient(_ftp.Host, _ftp.User, _ftp.Password);
//        client.EncryptionMode = FtpEncryptionMode.Explicit;
//        client.ValidateAnyCertificate = true;
//        await client.ConnectAsync();
//        await client.DeleteFileAsync($"{_ftp.RemotePath}/{filename}");
//    }
//}


public class FtpService : IFtpService
{
    public async Task<IEnumerable<string>> ListFilesAsync(FtpSetting setting)
    {
        using var client = CreateClient(setting);
        await client.ConnectAsync();
        var fullPaths = await client.GetNameListingAsync(setting.RemotePath);
        return fullPaths.Select(p => Path.GetFileName(p)).Where(name => name != null)!;
    }

    public async Task<Stream> DownloadAsync(string filename, FtpSetting setting)
    {
        var stream = new MemoryStream();
        using var client = CreateClient(setting);
        await client.ConnectAsync();
        await client.DownloadAsync(stream, $"{setting.RemotePath}/{filename}");

        stream.Position = 0;
        var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "FtpFiles");
        Directory.CreateDirectory(projectFolder);
        var localFilePath = Path.Combine(projectFolder, filename);

        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
        stream.Position = 0;
        return stream;
    }

    public async Task DeleteAsync(string filename, FtpSetting setting)
    {
        using var client = CreateClient(setting);
        await client.ConnectAsync();
        await client.DeleteFileAsync($"{setting.RemotePath}/{filename}");
    }

    private FtpClient CreateClient(FtpSetting setting)
    {
        var client = new FtpClient(setting.Host, setting.User, setting.Password)
        {
            EncryptionMode = FtpEncryptionMode.Explicit,
            ValidateAnyCertificate = true
        };
        return client;
    }
}
