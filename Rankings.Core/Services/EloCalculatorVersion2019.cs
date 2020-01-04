using System;

namespace Rankings.Core.Services
{
    public class EloCalculatorVersion2019 : IEloCalculator
    {
        public decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = ActualResult(gameScore1, gameScore2);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var marginOfVictoryMultiplier =  MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff);

            var outcome1 = (actualResult - expectedOutcome1);

            return 50  * outcome1 * marginOfVictoryMultiplier;
        }

        private decimal ActualResult(int gameScore1, int gameScore2)
        {
            if (gameScore1 == gameScore2)
                return 0.5m;

            return gameScore1 > gameScore2 ? 1 : 0;
        }

        public decimal MarginOfVictoryMultiplier(int gameScore1, int gameScore2, decimal winnerEloDiff)
        {
            if (gameScore1 == gameScore2)
                return 1;

            return (decimal)Math.Log(Math.Abs(gameScore1 - gameScore2) + 1) *
                   (2.2m / (winnerEloDiff * 0.001m + 2.2m));
        }

        private decimal CalculateExpectation(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            return ExpectationForWinningOneSet(ratingPlayer1, ratingPlayer2);
        }

        private decimal ExpectationForWinningOneSet(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            decimal exponent = (ratingPlayer2 - ratingPlayer1) / 400;
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
    
    public class EloCalculatorVersion2020 : IEloCalculator
    {
        public decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = ActualResult(gameScore1, gameScore2);

            var kfactor = ResolveKFactorFor(ratingPlayer1, gameScore1, gameScore2);
            var outcome1 = (actualResult - expectedOutcome1);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var delta = kfactor * outcome1;// * MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff);

            return delta;
        }

        private decimal ResolveKFactorFor(decimal rating, int gameScore1, int gameScore2)
        {
            var maxKfactor = ResolveMaxKFactor(gameScore1, gameScore2);
            
            if (rating > 1600)
                return (0.5m * maxKfactor).Round();

            if (rating > 1400)
                return (0.75m * maxKfactor).Round();

            return maxKfactor;
        }

        private int ResolveMaxKFactor(int gameScore1, int gameScore2)
        {
            var maxGameScore = Math.Max(gameScore1, gameScore2);
            if (maxGameScore >= 3)
                return 50;

            if (maxGameScore == 2)
                return 25;

            return 10;
        }

        private decimal ActualResult(int gameScore1, int gameScore2)
        {
            if (gameScore1 == gameScore2)
                return 0.5m;

            return gameScore1 > gameScore2 ? 1 : 0;
        }

        public decimal MarginOfVictoryMultiplier(int gameScore1, int gameScore2, decimal winnerEloDiff)
        {
            if (gameScore1 == gameScore2)
                return 1;

            return (decimal)Math.Log(Math.Sqrt(Math.Abs(gameScore1 - gameScore2) + 1)) *
                   (2.2m / (winnerEloDiff * 0.001m + 2.2m));
        }

        private decimal CalculateExpectation(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            return ExpectationForWinningOneSet(ratingPlayer1, ratingPlayer2);
        }

        private decimal ExpectationForWinningOneSet(decimal ratingPlayer1, decimal ratingPlayer2)
        {
            decimal exponent = (ratingPlayer2 - ratingPlayer1) / 400;
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }

}