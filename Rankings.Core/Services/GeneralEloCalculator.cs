using System;

namespace Rankings.Core.Services
{
    public static class GeneralEloCalculator
    {
        public static decimal ExpectationOneSet(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            decimal n = 400;
            decimal x = oldRatingPlayer1 - oldRatingPlayer2;
            decimal exponent = -1 * (x / n);
            decimal expected = (decimal) (1 / (1 + Math.Pow(10, (double) exponent)));

            return expected;
        }

        public static decimal MarginOfVictoryMultiplier(int gameScore1, int gameScore2, decimal winnerEloDiff)
        {
            if (gameScore1 == gameScore2)
                return 1;

            return (decimal) Math.Log(Math.Abs(gameScore1 - gameScore2) + 1) * (2.2m / (winnerEloDiff * 0.001m + 2.2m));
        }

        public static decimal ActualResult(int gameScore1, int gameScore2)
        {
            if (gameScore1 == gameScore2)
                return 0.5m;

            return gameScore1 > gameScore2 ? 1 : 0;
        }

        public static decimal CalculateExpectationForBestOf(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int numberOfGames)
        {
            var expectationOneSet = ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);

            decimal total = 0;
            for (var scorePlayer1 = numberOfGames; scorePlayer1 >= 0; --scorePlayer1)
            {
                var scoreOtherPlayer = numberOfGames - scorePlayer1;
                if (scorePlayer1 < scoreOtherPlayer)
                    break;
                var factor = scorePlayer1 == scoreOtherPlayer ? 0.5m : 1;

                total += factor * ChanceOfHavingThisResultInBestOf(scorePlayer1, scoreOtherPlayer, expectationOneSet);
            }

            return total;
        }

        public static decimal CalculateExpectationForResult(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int score1, int score2)
        {
            var expectationOneSet = ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);

            return ChanceOfHavingThisResultAllSetsPlayed(score1, score2, expectationOneSet);
        }

        public static decimal ChanceOfHavingThisResultAllSetsPlayed(int gameScore1, int gameScore2, decimal expectedToWinSet)
        {
            var numberOfSets = gameScore1 + gameScore2;
            var factorial = Factorial(numberOfSets) / (decimal)(Factorial(gameScore1) * Factorial(gameScore2));

            return factorial * (decimal)(Math.Pow((double) (1 - expectedToWinSet), gameScore2)
                             * Math.Pow((double) expectedToWinSet, gameScore1));
        }

        public static decimal ChanceOfHavingThisResultInBestOf(int gameScore1, int gameScore2, decimal expectedToWinSet)
        {
            var numberOfSets = gameScore1 + gameScore2;
            var factorial = Factorial(numberOfSets) / (double)(Factorial(gameScore1) * Factorial(gameScore2));
            var chance = Math.Pow((double) expectedToWinSet, gameScore1)
                         * Math.Pow((double) (1 - expectedToWinSet), gameScore2);

            var changeWinningGameWithSpecifiedResult = factorial * chance;

            return (decimal)changeWinningGameWithSpecifiedResult;
        }


        private static int Factorial(int numberOfSets)
        {
            if (numberOfSets == 0)
                return 1;

            if (numberOfSets == 1)
                return 1;

            return numberOfSets * Factorial(numberOfSets - 1);
        }
    }
}