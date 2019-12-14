using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Rankings.Core.Services;
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
        public void TestMargin(decimal elo1, decimal elo2, int score1, int score2, decimal expectedMultiplier)
        {
            var diff = score1 > score2 ? elo1 - elo2 : elo2 - elo1;
            var config = new EloConfiguration(50, 400, true, 1200);

            var actualMultiplier = new EloCalculator(config, new Mock<ILogger<EloCalculator>>().Object).MarginOfVictoryMultiplier(score1, score2, diff);

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

        [InlineData(1300, 1200, 2, 1, 11.932)]
        [InlineData(1300, 1200, 2, 0, 18.911)]
        [InlineData(1300, 1200, 3, 0, 23.864)]
        [InlineData(1300, 1200, 3, 1, 18.911)]
        [InlineData(1300, 1200, 3, 2, 11.932)]
        public void CalculateDeltaPlayerTests(decimal eloPlayerOne, decimal eloPlayerTwo, int scorePlayerOne, int scorePlayerTwo, decimal expectedDelta)
        {
            var logger = new Mock<ILogger<EloCalculator>>().Object;
            EloConfiguration config = new EloConfiguration(50, 400, true, 1200);
            var actualDelta = new EloCalculator(config, logger).CalculateDeltaPlayer(eloPlayerOne, eloPlayerTwo, scorePlayerOne, scorePlayerTwo);

            actualDelta.Should().BeApproximately(expectedDelta, 0.001m);
        }

        [Theory]
        [InlineData(1200, 1200, 1, 0.500)]
        [InlineData(1600, 1200, 1, 0.909)]
        [InlineData(1600, 1200, 3, 0.976)]
        [InlineData(1600, 1200, 5, 0.993)]
        public void CalculateExpectationTests(decimal eloPlayerOne, decimal eloPlayerTwo, int numberOfGames, decimal expected)
        {
            var actual = NewEloCalculator.CalculateExpectationForBestOf(eloPlayerOne, eloPlayerTwo, numberOfGames);

            actual.Should().BeApproximately(expected, 0.001m);
        }

        [Theory]
        [InlineData(1, 0, 0.8, 0.8)]
        [InlineData(2, 0, 0.8, 0.64)]
        [InlineData(2, 1, 0.8, 0.384)]
        [InlineData(1, 2, 0.8, 0.096)]
        [InlineData(0, 2, 0.8, 0.04)]
        [InlineData(0, 1, 0.8, 0.2)]
        [InlineData(1, 1, 0.8, 0.32)]
        public void WhenPlayingBestOfThree(int score1, int score2, decimal chanceWinningSet, decimal expectedChanceWinningGame)
        {
            var actualChanceWinningGame = NewEloCalculator.ChanceOfHavingThisResultAllSetsPlayed(score1, score2, chanceWinningSet);

            actualChanceWinningGame.Should().BeApproximately(expectedChanceWinningGame, 0.001m);
        }

        [Fact]
        public void WhenPlayingBestOfOneTheTotalChanceShouldAlwaysBeOne()
        {
            decimal eloPlayerOne = 1600;
            decimal eloPlayerTwo = 1200;
            var r1 = NewEloCalculator.CalculateExpectationForResult(eloPlayerOne, eloPlayerTwo, 1, 0);
            var r2 = NewEloCalculator.CalculateExpectationForResult(eloPlayerOne, eloPlayerTwo, 0, 1);

            (r1 + r2).Should().BeApproximately(1, 0.001m);
        }

        [Fact]
        public void WhenPlayingABestOfThreeAndAllSetsArePlayedTheTotalChanceShouldAlwaysBeOne()
        {
            var r1 = NewEloCalculator.ChanceOfHavingThisResultAllSetsPlayed(3, 0, 0.8m);
            var r2 = NewEloCalculator.ChanceOfHavingThisResultAllSetsPlayed(2, 1, 0.8m);
            var r3 = NewEloCalculator.ChanceOfHavingThisResultAllSetsPlayed(1, 2, 0.8m);
            var r4 = NewEloCalculator.ChanceOfHavingThisResultAllSetsPlayed(0, 3, 0.8m);

            (r1 + r2 + r3 + r4).Should().BeApproximately(1, 0.001m);
        }
    }
}