using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Aggregates.Games
{
    public class RegisterSingleGameCommandConsumer : IConsumer<RegisterSingleGameCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<RegisterSingleGameCommandConsumer> _logger;

        public RegisterSingleGameCommandConsumer()
        {
        }

        public RegisterSingleGameCommandConsumer(IEventStore eventStore, ILogger<RegisterSingleGameCommandConsumer> logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RegisterSingleGameCommand> context)
        {
            var command = context.Message;
            
            _logger.LogInformation("Consume command {@Command}", command);

            // TODO PBI check if game with id already exists

            // TODO PBI Check all fields
            
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