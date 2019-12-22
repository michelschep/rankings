using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Core.Specifications;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Xunit;

namespace Rankings.UnitTests
{
    public class RankingServiceIntegrationTests
    {
        private readonly IGamesService _gamesService;
        private readonly IStatisticsService _statisticsService;

        public RankingServiceIntegrationTests()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _gamesService = new GamesService(repository);
            var logger1 = (new Mock<ILogger<OldStatisticsService>>()).Object;
            var logger2 = (new Mock<ILogger<EloCalculator>>()).Object;

            var eloConfiguration = new EloConfiguration(50, 400, true, 1200);
            _statisticsService = new OldStatisticsService(_gamesService, eloConfiguration, logger1, new EloCalculator(eloConfiguration, logger2));
        }

        [Fact]
        public void WhenNoGamesTheRankingIsEmpty()
        {
            var ranking = _statisticsService.Ranking("tafeltennis", DateTime.MinValue, DateTime.MaxValue);

            Assert.True(!ranking.PlayerStats().Any());
        }

        [Fact]
        public void CanCreatePlayer()
        {
            var expectedPlayer = new Profile
            {
                EmailAddress = "email@address.nl",
                DisplayName = "Display Name"
            };
            _gamesService.CreateProfile(expectedPlayer);
            var actualPlayer = _gamesService.List(new SpecificProfile("EMAIL@ADDRESS.NL")).SingleOrDefault();

            // Assert
            actualPlayer.Should().Be(expectedPlayer);
        }

        [Fact]
        public void WhenPlayerCannotBeFound()
        {
            var actualPlayer = _gamesService.List(new SpecificProfile("does-not-exist@domain.nl")).SingleOrDefault();

            // Assert
            actualPlayer.Should().Be(null);
        }

        [Fact]
        public void CannotCreateGameWithoutPlayer1()
        {
            var emptyGame = new Game();
            
            // Assert
            _gamesService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>();
        }

        [Fact]
        public void CannotCreateGameWithoutPlayer2()
        {
            var player1 = new Profile
            {
                EmailAddress = "player1@domail.nl",
                DisplayName = "Player1"
            };
            _gamesService.CreateProfile(player1);

            var emptyGame = new Game
            {
                Player1 = player1
            };

            // Assert
            _gamesService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>();
        }

        [Fact]
        public void CannotCreateGameWithUnknownPlayer1()
        {
            var emptyGame = new Game
            {
                Player1 = new Profile()
            };

            // Assert
            _gamesService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>();
        }

        [Fact]
        public void CannotCreateGameWithUnknownPlayer2()
        {
            var player1 = new Profile
            {
                EmailAddress = "player1@domail.nl",
                DisplayName = "Player1"
            };
            _gamesService.CreateProfile(player1);

            var emptyGame = new Game
            {
                Player1 = player1,
                Player2 = new Profile()
            };

            // Assert
            _gamesService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(1, 1, 1200, 1200)]
        [InlineData(1, 0, 1217, 1183)]
        [InlineData(2, 0, 1227, 1173)]
        [InlineData(3, 0, 1235, 1165)]
        [InlineData(10, 0, 1260, 1140)]
        [InlineData(100, 0, 1315, 1085)]
        [InlineData(0, 1, 1183, 1217)]
        [InlineData(0, 2, 1173, 1227)]
        [InlineData(0, 3, 1165, 1235)]
        public void WhenFirstGameIsVictory(int score1, int score2, int expectedElo1, int expectedElo2)
        {
            // Arrange
            _gamesService.CreateProfile(CreateProfile("One"));
            _gamesService.CreateProfile(CreateProfile("Two"));
            _gamesService.CreateGameType(CreateTafeltennisGameType());

            // Act
            _gamesService.RegisterGame(CreateTafeltennisGame("One", "Two", score1, score2));
            var ranking = _statisticsService.Ranking("tafeltennis", DateTime.MinValue, DateTime.MaxValue);

            // Assert
            ranking.ForPlayer("one@domain.nl").Ranking.Round().Should().BeApproximately(expectedElo1, 0);
            ranking.ForPlayer("two@domain.nl").Ranking.Round().Should().BeApproximately(expectedElo2, 0);
        }

        private Game CreateTafeltennisGame(string player1, string player2, int score1, int score2)
        {
            var gameType = _gamesService.List(new SpecificGameType("tafeltennis")).Single();

            return new Game
            {
                GameType = gameType,
                Player1 = _gamesService.List(new SpecificProfile($"{player1}@domain.nl")).Single(),
                Player2 = _gamesService.List(new SpecificProfile($"{player2}@domain.nl")).Single(),
                Score1 = score1,
                Score2 = score2
            };
        }

        private static Profile CreateProfile(string name)
        {
            return new Profile
            {
                EmailAddress = $"{name}@domain.nl",
                DisplayName = name
            };
        }

        private static GameType CreateTafeltennisGameType()
        {
            return new GameType
            {
                Code = "tafeltennis",
                DisplayName = "Tafeltenis"
            };
        }
    }
}
