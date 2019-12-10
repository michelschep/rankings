using System;
using System.Linq;
using FluentAssertions;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Core.Specifications;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Xunit;

namespace Rankings.UnitTests
{
    public class AcceptanceTests
    {
        private readonly GamesService _gamesService;
        private readonly StatisticsService _statisticsService;
        private readonly Venue _venue;
        private readonly GameType _gameType;

        public AcceptanceTests()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _gamesService = new GamesService(repository);
            var eloConfiguration = new EloConfiguration(5, 50, false, 100);
            _statisticsService = new StatisticsService(_gamesService, eloConfiguration);
            _venue = new Venue {Code = "almere", DisplayName = "Almere Arena"};
            _gamesService.CreateVenue(_venue);
            _gameType = new GameType {Code = "tafeltennis", DisplayName = "Tafeltennis"};
            _gamesService.CreateGameType(_gameType);
        }

        [Fact]
        public void Test()
        {
            _gamesService.ActivateProfile("amy@domain.nl", "Amy");
            _gamesService.ActivateProfile("brad@domain.nl", "Brad");
            _gamesService.ActivateProfile("cindy@domain.nl", "Cindy");
            _gamesService.ActivateProfile("dirk@domain.nl", "Dirk");

            var amy = _gamesService.List(new SpecificProfile("amy@domain.nl")).Single();
            var brad = _gamesService.List(new SpecificProfile("brad@domain.nl")).Single();
            var cindy = _gamesService.List(new SpecificProfile("cindy@domain.nl")).Single();
            var dirk = _gamesService.List(new SpecificProfile("dirk@domain.nl")).Single();

            _gamesService.RegisterGame(CreateGame(amy, brad, 1, 0));
            _gamesService.RegisterGame(CreateGame(dirk, cindy, 1, 0));
            _gamesService.RegisterGame(CreateGame(amy, cindy, 1, 0));
            _gamesService.RegisterGame(CreateGame(dirk, cindy, 1, 0));

            var ranking = _statisticsService.Ranking("tafeltennis");

            ranking.ForPlayer(amy.EmailAddress).Ranking.Should().Be(104.71m);
            ranking.ForPlayer(dirk.EmailAddress).Ranking.Should().Be(104.59m);
            ranking.ForPlayer(brad.EmailAddress).Ranking.Should().Be(97.5m);
            ranking.ForPlayer(cindy.EmailAddress).Ranking.Should().Be(93.20m);
        }

        private Game CreateGame(Profile playerOne, Profile playerTwo, int score1, int score2)
        {
            return new Game {Venue = _venue, GameType = _gameType, Player1 = playerOne, Player2 = playerTwo, Score1 = score1, Score2 = score2};
        }
    }
}