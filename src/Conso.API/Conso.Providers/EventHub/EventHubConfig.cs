namespace Conso.Providers.EventHub
{
    public class EventHubConfig
    {
        public required string HubName { get; set; }
        public required string ConnectionString { get; set; }
    }
}
