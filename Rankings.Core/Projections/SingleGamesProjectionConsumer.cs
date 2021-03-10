using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Aggregates.Games;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;

namespace Rankings.Core.Projections
{
    public class SingleGamesProjectionConsumer : IConsumer<SingleGameRegistered>
    {
        private readonly IGamesProjection _gamesProjection;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<SingleGamesProjectionConsumer> _logger;

        public SingleGamesProjectionConsumer()
        {
        }

        public SingleGamesProjectionConsumer(IGamesProjection gamesProjection, IPublishEndpoint publisher, ILogger<SingleGamesProjectionConsumer> logger)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SingleGameRegistered> context)
        {
            _logger.LogInformation("Consume event {@Event}", context.Message);

            var profile1 = _gamesProjection.Item(new SpecificProfile(context.Message.FirstPlayer));
            var profile2 = _gamesProjection.Item(new SpecificProfile(context.Message.SecondPlayer));
            
            var game = new GameProjection
            {
                Identifier = context.Message.Identifier.ToString(),
                RegistrationDate = context.Message.RegistrationDate,
                GameType = context.Message.GameType,
                Venue = _gamesProjection.Item(new SpecificVenue(context.Message.Venue)).DisplayName,
                FirstPlayerId = profile1.Identifier,
                FirstPlayerName = profile1.DisplayName,
                SecondPlayerId = profile2.Identifier,
                SecondPlayerName = profile2.DisplayName,
                Score1 = context.Message.ScoreFirstPlayer,
                Score2 = context.Message.ScoreSecondPlayer,
                Status = "Not approved yet",
                EloFirstPlayer = "",
                EloSecondPlayer = ""
            };

            _gamesProjection.RegisterGame(game);
        }
    }
}