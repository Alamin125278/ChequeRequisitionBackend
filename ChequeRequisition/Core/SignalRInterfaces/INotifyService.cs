namespace ChequeRequisiontService.Core.SignalRInterfaces;

public interface INotifyService
{
    public Task Notify<T>(string eventName, T data);
}
