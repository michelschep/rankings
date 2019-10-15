using FluentAssertions;
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
        //[InlineData(3200, 1000, 0, 1, 15.249)]
        //[InlineData(3201, 1000, 0, 1, -1524.923)]

        //[InlineData(1200, 1200, 2, 0, 1.098)]
        //[InlineData(1300, 1200, 2, 0, 1.050)]
        //[InlineData(1300, 1200, 3, 0, 1.326)]
        public void TestMargin(decimal elo1, decimal elo2, int score1, int score2, decimal expectedMultiplier)
        {
            var diff = score1 > score2 ? elo1 - elo2 : elo2 - elo1;
            var actualMultiplier = EloCalculator.MarginOfVictoryMultiplier(score1, score2, diff);

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
        public void CalculateDeltaPlayerTests(decimal elo1, decimal elo2, int score1, int score2, decimal expectedDelta)
        {
            var actualDelta = EloCalculator.CalculateDeltaPlayer(elo1, elo2, score1, score2);

            actualDelta.Should().BeApproximately(expectedDelta, 0.001m);
        }

        [Theory]
        [InlineData(1200, 1200, 1, 0.500)]
        //[InlineData(1200, 1200, 2, 0.500)]
        //[InlineData(1200, 1200, 3, 0.500)]
        //[InlineData(1200, 1200, 5, 0.500)]
        [InlineData(1600, 1200, 1, 0.909)]
        [InlineData(1600, 1200, 3, 0.976)]
        [InlineData(1600, 1200, 5, 0.993)]
        public void CalculateExpectationTests(decimal elo1, decimal elo2, int numberOfGames, decimal expected)
        {
            var actual = NewEloCalculator.CalculateExpectation(elo1, elo2, numberOfGames);

            actual.Should().BeApproximately(expected, 0.001m);
        }
    }
}