using System;
using FluentAssertions;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Xunit;

namespace Rankings.UnitTests
{
    public class AcceptanceTests
    {
        private readonly RankingService _rankingService;
        private readonly Venue _venue;
        private readonly GameType _gameType;

        public AcceptanceTests()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _rankingService = new RankingService(repository, 100, 5, 50, false, 2);
            _venue = new Venue {Code = "almere", DisplayName = "Almere Arena"};
            _rankingService.CreateVenue(_venue);
            _gameType = new GameType {Code = "tafeltennis", DisplayName = "Tafeltennis"};
            _rankingService.CreateGameType(_gameType);
        }

        [Fact]
        public void Test()
        {
            _rankingService.ActivateProfile("amy@domain.nl", "Amy");
            _rankingService.ActivateProfile("brad@domain.nl", "Brad");
            _rankingService.ActivateProfile("cindy@domain.nl", "Cindy");
            _rankingService.ActivateProfile("dirk@domain.nl", "Dirk");

            var amy = _rankingService.ProfileFor("amy@domain.nl");
            var brad = _rankingService.ProfileFor("brad@domain.nl");
            var cindy = _rankingService.ProfileFor("cindy@domain.nl");
            var dirk = _rankingService.ProfileFor("dirk@domain.nl");

            _rankingService.RegisterGame(CreateGame(amy, brad, 1, 0));
            _rankingService.RegisterGame(CreateGame(dirk, cindy, 1, 0));
            _rankingService.RegisterGame(CreateGame(amy, cindy, 1, 0));
            _rankingService.RegisterGame(CreateGame(dirk, cindy, 1, 0));

            var ranking = _rankingService.Ranking("tafeltennis");

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