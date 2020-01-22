using System;

namespace Rankings.Core.Services
{
    public class DefaultEloCalculator : IEloCalculator
    {
        public decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = ActualResult(gameScore1, gameScore2);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var outcome1 = (actualResult - expectedOutcome1);

            return 5  * outcome1;
        }

        private decimal ActualResult(int gameScore1, int gameScore2)
        {
            if (gameScore1 == gameScore2)
                return 0.5m;

            return gameScore1 > gameScore2 ? 1 : 0;
        }

        private decimal CalculateExpectation(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            decimal exponent = (ratingPlayer2 - ratingPlayer1) / 50;
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
}