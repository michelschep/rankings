using System;
using System.Linq;
using FluentAssertions;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Xunit;

namespace Rankings.UnitTests
{
    public class EloCalculatorTests
    {
        [Theory]
        [InlineData(1200, 1200, 1, 0, 0.693)]
        [InlineData(1300, 1200, 1, 0, 0.663)]
        [InlineData(1400, 1200, 1, 0, 0.635)]
        [InlineData(2000, 1000, 1, 0, 0.476)]
        [InlineData(3000, 1000, 1, 0, 0.363)]
        [InlineData(4000, 1000, 1, 0, 0.293)]
        [InlineData(8000, 1000, 1, 0, 0.165)]
        [InlineData(10000, 1000, 1, 0, 0.136)]
        [InlineData(20000, 1000, 1, 0, 0.071)]
        [InlineData(100000, 1000, 1, 0, 0.015)]

        [InlineData(2000, 1000, 0, 1, 1.270)]
        [InlineData(3000, 1000, 0, 1, 7.624)]
        [InlineData(3100, 1000, 0, 1, 15.249)]
        [InlineData(3199, 1000, 0, 1, 1524.923)]
        //[InlineData(3200, 1000, 0, 1, 15.249)]
        [InlineData(3201, 1000, 0, 1, -1524.923)]

        [InlineData(1200, 1200, 2, 0, 1.098)]
        [InlineData(1300, 1200, 2, 0, 1.050)]
        [InlineData(1300, 1200, 3, 0, 1.326)]
        public void Test(decimal elo1, decimal elo2, int score1, int score2, decimal expectedMultiplier)
        {
            var actualMultiplier = EloCalculator.MarginOfVictoryMultiplier(elo1, elo2, score1, score2);

            actualMultiplier.Should().BeApproximately(expectedMultiplier, 0.001m);
        }

        [Theory]
        [InlineData(1200, 1200, 1, 0, 17.328)]
        [InlineData(1200, 1000, 1, 0, 7.632)]
        [InlineData(1400, 1000, 1, 0, 2.665)]
        [InlineData(2000, 1000, 1, 0, 0.075)]
        [InlineData(4000, 1000, 1, 0, 0.000)]
        [InlineData(8000, 1000, 1, 0, 0.000)]

        [InlineData(1200, 1200, 0, 1, -17.328)]
        [InlineData(1200, 1000, 0, 1, -28.963)]
        [InlineData(1400, 1000, 0, 1, -38.508)]
        [InlineData(2000, 1000, 0, 1, -63.338)]
        //[InlineData(4000, 1000, 0, 1, -80)]
        public void CalculateDeltaPlayerTests(decimal elo1, decimal elo2, int score1, int score2, decimal expectedDelta)
        {
            var actualDelta = EloCalculator.CalculateDeltaPlayer(elo1, elo2, score1, score2);

            actualDelta.Should().BeApproximately(expectedDelta, 0.001m);
        }

        [Theory]
        [InlineData(1200, 1200, 1, 0.500)]
        [InlineData(1200, 1200, 2, 0.500)]
        [InlineData(1200, 1200, 3, 0.500)]
        [InlineData(1200, 1200, 5, 0.500)]
        [InlineData(1600, 1200, 1, 0.909)]
        [InlineData(1600, 1200, 3, 0.976)]
        [InlineData(1600, 1200, 5, 0.993)]
        public void CalculateExpectationTests(decimal elo1, decimal elo2, int numberOfGames, decimal expected)
        {
            var actual = EloCalculator.CalculateExpectation(elo1, elo2, numberOfGames);

            actual.Should().BeApproximately(expected, 0.001m);
        }
    }

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
            var player1 = new Profile
            {
                EmailAddress = "player1@domail.nl"
            };
            _rankingService.CreateProfile(player1);

            var emptyGame = new Game
            {
                Player1 = player1
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
                .WithMessage("Cannot register game because player1 is not registered");
        }

        [Fact]
        public void CannotCreateGameWithUnknownPlayer2()
        {
            var player1 = new Profile
            {
                EmailAddress = "player1@domail.nl"
            };
            _rankingService.CreateProfile(player1);

            var emptyGame = new Game
            {
                Player1 = player1,
                Player2 = new Profile()
            };

            // Assert
            _rankingService.Invoking(r => r.RegisterGame(emptyGame))
                .Should().Throw<Exception>()
                .WithMessage("Cannot register game because player2 is not registered");
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
