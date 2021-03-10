using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rankings.Core.Aggregates.Games;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;

namespace Rankings.Infrastructure.Data
{
    public class EventStore : IEventStore
    {
        private readonly RankingContext _dbContext;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<EventStore> _logger;

        public EventStore(RankingContext dbContext, IPublishEndpoint publisher, ILogger<EventStore> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateEvent(SingleGameRegistered singleGameRegistered)
        {
            _logger.LogInformation("Create event {Event}", singleGameRegistered);

            var body = JsonConvert.SerializeObject(singleGameRegistered);

            var index = Index();

            if (index == 0)
            {
                index = 1;
                foreach (var g in _dbContext.Games.Include(game => game.Player1).Include(game=>game.Player2).Include(game=>game.Venue).Include(game=>game.GameType).OrderBy(game => game.RegistrationDate))
                {
                    var sgr = new SingleGameRegistered
                    {
                        FirstPlayer = Guid.Parse(g.Player1.Identifier),
                        SecondPlayer = Guid.Parse(g.Player2.Identifier),
                        Identifier = Guid.NewGuid(),
                        Venue = g.Venue.Code,
                        GameType = g.GameType.Code,
                        RegistrationDate = g.RegistrationDate,
                        ScoreFirstPlayer = g.Score1,
                        ScoreSecondPlayer = g.Score2,
                        SetScoresFirstPlayer = g.SetScores1,
                        SetScoresSecondPlayer = g.SetScores2
                    };
                        
                    await _dbContext.Events.AddAsync(new Event
                    {
                        Identifier = Guid.NewGuid().ToString(),
                        Index = index++,
                        CreationDate = g.RegistrationDate,
                        Type = nameof(SingleGameRegistered),
                        Body = JsonConvert.SerializeObject(sgr)
                    });
                }
            }
            else
            {
                index++;
            }
            
            await _dbContext.Events.AddAsync(new Event
            {
                Identifier = Guid.NewGuid().ToString(),
                Index = index,
                CreationDate = DateTime.UtcNow,
                Type = nameof(SingleGameRegistered),
                Body = body
            });
            await _dbContext.SaveChangesAsync();
            await _publisher.Publish(singleGameRegistered);
        }

        private int Index()
        {
            if (_dbContext.Events.Any())
            {
                return _dbContext.Events.Select(e => e.Index).Max();
            }

            return 0;
        }
    }
}