using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Commands
{
    public class RegisterSingleGameConsumer : IConsumer<RegisterSingleGameCommand>
    {
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<RegisterSingleGameConsumer> _logger;

        public RegisterSingleGameConsumer()
        {
        }

        public RegisterSingleGameConsumer(IGamesService gamesService, IPublishEndpoint publisher, ILogger<RegisterSingleGameConsumer> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RegisterSingleGameCommand> context)
        {
            _logger.LogInformation("Consume message {message}", context.Message);

            await _publisher.Publish(new SingleGameRegistered
            {
                Identifier = Guid.NewGuid(),
                RegistrationDate = context.Message.RegistrationDate,
                Venue = context.Message.Venue,
                GameType = context.Message.GameType,
                FirstPlayer = context.Message.FirstPlayer,
                SecondPlayer = context.Message.SecondPlayer,
                ScoreFirstPlayer = context.Message.ScoreFirstPlayer,
                ScoreSecondPlayer = context.Message.ScoreSecondPlayer
            });
        }
    }
}