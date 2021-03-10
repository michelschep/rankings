using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Commands
{
    public class RegisterSingleGameCommandConsumer : IConsumer<RegisterSingleGameCommand>
    {
        private readonly IGamesProjection _gamesProjection;
        private readonly IRepository _repository;
        private readonly IPublishEndpoint _publisher;
        private readonly IEventStore _eventStore;
        private readonly ILogger<RegisterSingleGameCommandConsumer> _logger;

        public RegisterSingleGameCommandConsumer()
        {
        }

        public RegisterSingleGameCommandConsumer(IGamesProjection gamesProjection, IRepository repository, IPublishEndpoint publisher, IEventStore eventStore, ILogger<RegisterSingleGameCommandConsumer> logger)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _publisher = publisher;
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RegisterSingleGameCommand> context)
        {
            var command = context.Message;
            
            _logger.LogInformation("Consume command {@Command}", command);

            if (command.GameType == null)
                throw new Exception("Cannot register game because game type is not specified");

            if (command.FirstPlayer == null)
                throw new Exception("Cannot register game because player1 is not specified");

            // TODO PBI
            //if (_repository.GetById<Profile>(command.FirstPlayer) == null)
            //    throw new Exception($"Cannot register game because player1 [{game.Player1.DisplayName}] is not registered");

            if (command.SecondPlayer == null)
                throw new Exception("Cannot register game because player2 is not specified");

            // TODO PBI
            //if (_repository.GetById<Profile>(game.Player2.Id) == null)
            //    throw new Exception("Cannot register game because player2 is not registered");

            var singleGameRegistered = new SingleGameRegistered
            {
                Identifier = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                Venue = command.Venue,
                GameType = command.GameType,
                FirstPlayer = command.FirstPlayer,
                SecondPlayer = command.SecondPlayer,
                ScoreFirstPlayer = command.ScoreFirstPlayer,
                SetScoresFirstPlayer = command.SetScoresFirstPlayer,
                ScoreSecondPlayer = command.ScoreSecondPlayer,
                SetScoresSecondPlayer = command.SetScoresSecondPlayer
            };

            await _eventStore.CreateEvent(singleGameRegistered);
        }
    }
}