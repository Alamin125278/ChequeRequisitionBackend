using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

public class FtpWorkerJobService(FtpSetting ftpSetting, IServiceScopeFactory scopeFactory, ILogger<FtpWorkerJobService> logger) : BackgroundService
{
    private readonly FtpSetting _ftpSetting = ftpSetting;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<FtpWorkerJobService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_logger.LogInformation($"[BANK {_ftpSetting.BankName}] Worker started.");
        using var outerScope = _scopeFactory.CreateScope();
        var logDb = outerScope.ServiceProvider.GetRequiredService<IFtpLogDbService>();
        await logDb.InsertFtpLogAsync(new FtpFileLogDto
        {
            BankName = _ftpSetting.BankName,
            LogLevel = "Information",
            Message = "Worker started."
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var ftpService = scope.ServiceProvider.GetRequiredService<IFtpService>();
            var parser = scope.ServiceProvider.GetRequiredService<IExcelParserService>();
            var repo = scope.ServiceProvider.GetRequiredService<IFtpImportRepo>();
            var logDbScope = scope.ServiceProvider.GetRequiredService<IFtpLogDbService>();
            var trackingRepo =scope.ServiceProvider.GetRequiredService<IRequisitionTrackingRepo>();

            try
            {
                var files = await ftpService.ListFilesAsync(_ftpSetting);

                foreach (var file in files)
                {
                    try
                    {
                        using var stream = await ftpService.DownloadAsync(file, _ftpSetting);
                        var items = (await parser.ParseAsync(stream,_ftpSetting)).ToList();

                        //foreach (var item in items)
                            //item.BankId = _ftpSetting.BankId;

                        var requisitionIds=await repo.BulkInsertRequisitionsAsync(items, stoppingToken);

                        var importLogId =await repo.InsertFtpLogAsync(new FtpImportLogDto
                        {
                            BankId = 3,
                            Filename = file,
                            ImportedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }, stoppingToken);

                        var trackings = requisitionIds.Select(id => new RequisitionImportTrackingDto
                        {
                            RequisitionId = id,
                            ImportLogId = importLogId
                        }).ToList();

                        await trackingRepo.InsertTrackingAsync(trackings, stoppingToken);

                        await ftpService.DeleteAsync(file, _ftpSetting);
                        //_logger.LogInformation($"[BANK {_ftpSetting.BankName}] Imported: {file}");
                        await logDbScope.InsertFtpLogAsync(new FtpFileLogDto
                        {
                            BankName = _ftpSetting.BankName,
                            LogLevel = "Information",
                            Message = $"Imported: {file}"
                        }, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, $"[BANK {_ftpSetting.BankName}] Failed: {file}");
                        await logDbScope.InsertFtpLogAsync(new FtpFileLogDto
                        {
                            BankName = _ftpSetting.BankName,
                            LogLevel = "Error",
                            Message = $"Failed: {file}",
                            Exception = ex.ToString()
                        }, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"[BANK {_ftpSetting.BankName}] Connection failed");
                await logDb.InsertFtpLogAsync(new FtpFileLogDto
                {
                    BankName = _ftpSetting.BankName,
                    LogLevel = "Error",
                    Message = "Connection failed",
                    Exception = ex.ToString()
                }, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
        }
    }
}

