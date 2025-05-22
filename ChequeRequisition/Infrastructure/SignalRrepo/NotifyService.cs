using ChequeRequisiontService.Core.SignalRInterfaces;
using ChequeRequisiontService.Infrastructure.SignalRrepo;
using Microsoft.AspNetCore.SignalR;


public class NotifyService(IHubContext<AppHub> hubContext) : INotifyService
{
    private readonly IHubContext<AppHub> _hubContext = hubContext;

    public async Task Notify<T>(string eventName, T message)
    {
        await _hubContext.Clients.All.SendAsync(eventName, message);
    }
}
