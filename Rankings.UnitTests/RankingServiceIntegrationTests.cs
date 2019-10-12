using System;
using System.Linq;
using FluentAssertions;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Xunit;

namespace Rankings.UnitTests
{
    public class RankingServiceIntegrationTests
    {
        private readonly RankingService _rankingService;

        public RankingServiceIntegrationTests()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _rankingService = new RankingService(repository);
        }

        [Fact]
        public void WhenNoGamesTheRankingIsEmpty()
        {
            var ranking = _rankingService.Ranking();

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
            _rankingService.CreateProfile(expectedPlayer);
            var actualPlayer = _rankingService.ProfileFor("EMAIL@ADDRESS.NL");

            // Assert
            actualPlayer.Should().Be(expectedPlayer);
        }

        [Fact]
        public void WhenPlayerCannotBeFound()
        {
            var actualPlayer = _rankingService.ProfileFor("does-not-exist@domain.nl");

            // Assert
            actualPlayer.Should().Be(null);
        }

        [Fact]
        public void CannotCreateGameWithoutPlayer1()
        {
            var emptyGame = new Game();
            
            // Assert
            _rankingService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>()
                .WithMessage("Cannot register game because player1 is not specified");
        }

        [Fact]
        public void CannotCreateGameWithoutPlayer2()
        {
            var emptyGame = new Game
            {
                Player1 = new Profile()
            };

            // Assert
            _rankingService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>()
                .WithMessage("Cannot register game because player2 is not specified");
        }

        [Fact]
        public void CannotCreateGameWithUnknownPlayer1()
        {
            var emptyGame = new Game
            {
                Player1 = new Profile()
            };

            // Assert
            _rankingService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>()
                .WithMessage("Cannot register game because player1 is unknown");
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
            _rankingService.CreateProfile(CreateProfile("One"));
            _rankingService.CreateProfile(CreateProfile("Two"));
            _rankingService.CreateGameType(CreateTafeltennisGameType());

            // Act
            _rankingService.RegisterGame(CreateTafeltennisGame("One", "Two", score1, score2));
            var ranking = _rankingService.Ranking();

            // Assert
            ranking.ForPlayer("one@domain.nl").Ranking.Should().Be(expectedElo1);
            ranking.ForPlayer("two@domain.nl").Ranking.Should().Be(expectedElo2);
        }

        private Game CreateTafeltennisGame(string player1, string player2, int score1, int score2)
        {
            var gameTypes = _rankingService.GameTypes();
            return new Game
            {
                GameType = gameTypes.Single(type => type.Code == "tafeltennis"),
                Player1 = _rankingService.ProfileFor($"{player1}@domain.nl"),
                Player2 = _rankingService.ProfileFor($"{player2}@domain.nl"),
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
