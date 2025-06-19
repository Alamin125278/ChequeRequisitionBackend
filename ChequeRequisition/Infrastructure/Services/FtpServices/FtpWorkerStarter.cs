using ChequeRequisiontService.Core.Dto.Ftp;
using Microsoft.Extensions.Options;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

public class FtpWorkerStarter(IServiceProvider serviceProvider, ILogger<FtpWorkerStarter> logger) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<FtpWorkerStarter> _logger = logger;
    private readonly List<IHostedService> _runningWorkers = [];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var ftpSettings = scope.ServiceProvider.GetRequiredService<IOptions<List<FtpSetting>>>().Value;

        foreach (var setting in ftpSettings)
        {
            var worker = new FtpWorkerJobService(setting,
                _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                _serviceProvider.GetRequiredService<ILogger<FtpWorkerJobService>>());

            _runningWorkers.Add(worker);
            _ = worker.StartAsync(cancellationToken); // fire & forget
            _logger.LogInformation($"Started FTP worker for BankId: {setting.BankName}");
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var worker in _runningWorkers)
        {
            await worker.StopAsync(cancellationToken);
        }
    }
}

