using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Commands
{
    public class SingleGameRegisteredConsumer : IConsumer<SingleGameRegistered>
    {
        private readonly IGamesService _gamesService;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<SingleGameRegisteredConsumer> _logger;

        public SingleGameRegisteredConsumer()
        {
        }

        public SingleGameRegisteredConsumer(IGamesService gamesService, IPublishEndpoint publisher, ILogger<SingleGameRegisteredConsumer> logger)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SingleGameRegistered> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Consume message {message}", @event);

            _gamesService.RegisterGame(new Game()
            {
                Identifier = @event.Identifier.ToString(),
                RegistrationDate = @event.RegistrationDate.UtcDateTime,
                Score1 = @event.ScoreFirstPlayer,
                Score2 = @event.ScoreSecondPlayer,
            });
        }
    }
}