using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Conso.Providers.Interfaces.EventHub;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Conso.Providers.EventHub
{
    public class EventHubProvider : IEventHubProvider, IDisposable
    {
        private readonly EventHubConfig _config;
        private readonly ILogger<EventHubProvider> _logger;
        private readonly EventHubProducerClient _producerClient;

        public EventHubProvider(IOptions<EventHubConfig> config, ILogger<EventHubProvider> logger)
        {
            _config = config.Value;
            _logger = logger;
            _producerClient = new EventHubProducerClient(_config.ConnectionString, _config.HubName);
        }

        public async void Dispose()
        {
            await _producerClient.DisposeAsync();
        }

        public async Task PushAsync(Message[] messages)
        {
            _logger.LogInformation("Message pushing.");

            if (messages.Length == 0)
            {
                return;
            }           

            // Create a batch of events 
            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();
            foreach (var message in messages)
            {
                string jsonString = JsonConvert.SerializeObject(message);
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonString))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event is too large for the batch and cannot be sent.");
                }
            }

            // Use the producer client to send the batch of events to the event hub
            await _producerClient.SendAsync(eventBatch);
        }
    }
}
