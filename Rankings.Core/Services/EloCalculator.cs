using System;

namespace Rankings.Core.Services
{
    public static class EloCalculator
    {
        public static decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var K = 50;

            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = ActualResult(gameScore1, gameScore2);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var marginOfVicoryMultiplier = MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff);
            //var marginOfVicoryMultiplier = 1;

            var outcome1 = (actualResult - expectedOutcome1);
            var player1Delta = K * outcome1 * marginOfVicoryMultiplier;

            return player1Delta;
        }

        private static decimal ActualResult(int gameScore1, int gameScore2)
        {
            if (gameScore1 == gameScore2)
                return 0.5m;

            return gameScore1 > gameScore2 ? 1 : 0;
        }

        public static decimal MarginOfVictoryMultiplier(int gameScore1, int gameScore2, decimal winnerEloDiff)
        {
            if (gameScore1 == gameScore2)
                return 1;

            return (decimal)Math.Log(Math.Abs(gameScore1 - gameScore2) + 1) *
                   (2.2m / (winnerEloDiff * 0.001m + 2.2m));
        }

        public static decimal CalculateExpectation(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            return ExpectationForWinningOneSet(ratingPlayer1, ratingPlayer2);
        }

        private static decimal ExpectationForWinningOneSet(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            decimal n = 400;
            decimal exponent = (ratingPlayer2 - ratingPlayer1) / n;
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
}