namespace Conso.Providers.Interfaces.EventHub
{
    public interface IEventHubProvider
    {
        Task PushAsync(Message[] messages);
    }
}
