using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Commands
{
    public class RegisterSingleGameCommandConsumer : IConsumer<RegisterSingleGameCommand>
    {
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<RegisterSingleGameCommandConsumer> _logger;

        public RegisterSingleGameCommandConsumer()
        {
        }

        public RegisterSingleGameCommandConsumer(IGamesService gamesService, IPublishEndpoint publisher, ILogger<RegisterSingleGameCommandConsumer> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RegisterSingleGameCommand> context)
        {
            _logger.LogInformation("Consume command {@Command}", context.Message);

            await _publisher.Publish(new SingleGameRegistered
            {
                Identifier = Guid.NewGuid(),
                RegistrationDate = context.Message.RegistrationDate,
                Venue = context.Message.Venue,
                GameType = context.Message.GameType,
                FirstPlayer = context.Message.FirstPlayer,
                SecondPlayer = context.Message.SecondPlayer,
                ScoreFirstPlayer = context.Message.ScoreFirstPlayer,
                SetScoresFirstPlayer = context.Message.SetScoresFirstPlayer,
                ScoreSecondPlayer = context.Message.ScoreSecondPlayer,
                SetScoresSecondPlayer = context.Message.SetScoresSecondPlayer
            });
        }
    }
}