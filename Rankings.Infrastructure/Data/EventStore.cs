using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Commands;

namespace Rankings.Infrastructure.Data
{
    public class EventStore : IEventStore
    {
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<EventStore> _logger;

        public EventStore(IPublishEndpoint publisher, ILogger<EventStore> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task CreateEvent(SingleGameRegistered singleGameRegistered)
        {
            _logger.LogInformation("Create event {Event}", singleGameRegistered);
            // TODO PBI store event in event store. Then publish.
            
            await _publisher.Publish(singleGameRegistered);
        }
    }
}