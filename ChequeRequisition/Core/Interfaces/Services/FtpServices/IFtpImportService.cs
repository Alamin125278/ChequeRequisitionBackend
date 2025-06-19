namespace ChequeRequisiontService.Core.Interfaces.Services.FtpServices;

public interface IFtpImportService
{
    Task RunImportAsync(CancellationToken cancellationToken);
}
