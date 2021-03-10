using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;

namespace Rankings.Core.Commands
{
    public class SingleGameRegisteredProjectionConsumer : IConsumer<SingleGameRegistered>
    {
        private readonly IGamesProjection _gamesProjection;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<SingleGameRegisteredProjectionConsumer> _logger;

        public SingleGameRegisteredProjectionConsumer()
        {
        }

        public SingleGameRegisteredProjectionConsumer(IGamesProjection gamesProjection, IPublishEndpoint publisher, ILogger<SingleGameRegisteredProjectionConsumer> logger)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SingleGameRegistered> context)
        {
            _logger.LogInformation("Consume event {@Event}", context.Message);

            var game = new Game
            {
                Identifier = context.Message.Identifier.ToString(),
                RegistrationDate = context.Message.RegistrationDate,
                GameType = _gamesProjection.Item(new SpecificGameType(context.Message.GameType)),
                Venue = _gamesProjection.Item(new SpecificVenue(context.Message.Venue)),
                Player1 = _gamesProjection.Item(new SpecificProfile(context.Message.FirstPlayer)),
                Player2 = _gamesProjection.Item(new SpecificProfile(context.Message.SecondPlayer)),
                Score1 = context.Message.ScoreFirstPlayer,
                Score2 = context.Message.ScoreSecondPlayer,
                SetScores1 = context.Message.SetScoresFirstPlayer,
                SetScores2 = context.Message.SetScoresSecondPlayer
            };

            _gamesProjection.RegisterGame(game);
        }
    }
}