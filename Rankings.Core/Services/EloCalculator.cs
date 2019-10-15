using System;

namespace Rankings.Core.Services
{
    public static class NewEloCalculator
    {
        public static decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var K = 50;
            decimal actualResult = ActualResult(gameScore1, gameScore2);
            var expectation = CalculateExpectation(ratingPlayer1, ratingPlayer2, gameScore1+gameScore2);
            //var expectation = CalculateExpectation2(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);
            var marginOfVictoryMultiplier = 1;//MarginOfVictoryMultiplier(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);

            var high = Math.Max(gameScore1, gameScore2);

            return (K + 5*(2*high-1)) * (actualResult - expectation) * marginOfVictoryMultiplier;
        }

        private static int ActualResult(int gameScore1, int gameScore2)
        {
            return gameScore1 > gameScore2 ? 1 : 0;
        }

        public static decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            return ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);
        }

        public static decimal MarginOfVictoryMultiplier(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var log = (decimal) Math.Log(Math.Abs(gameScore1 - gameScore2) + 1);
            var eloDiff = (2.2m / (winnerEloDiff * 0.001m + 2.2m));

            return log * eloDiff;
        }

        public static decimal ExpectationOneSet(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            decimal n = 400;
            decimal x = oldRatingPlayer1 - oldRatingPlayer2;
            decimal exponent = -1 * (x / n);
            decimal expected = (decimal) (1 / (1 + Math.Pow(10, (double) exponent)));

            return expected;
        }

        public static decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int numberOfGames)
        {
            var expectationOneSet = ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);

            decimal total = 0;
            decimal index = 0;
            for (var scorePlayer1 = numberOfGames; scorePlayer1 >= 0; --scorePlayer1)
            {
                var scoreOtherPlayer = numberOfGames - scorePlayer1;
                if (scorePlayer1 < scoreOtherPlayer)
                    break;
                var factor = scorePlayer1 == scoreOtherPlayer ? 0.5m : 1;

                total += factor * ChanceOfHavingThisResult(scorePlayer1, scoreOtherPlayer, expectationOneSet);
                ++index;
            }

            return total;
        }

        public static decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int score1, int score2)
        {
            var expectationOneSet = ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);

            return ChanceOfHavingThisResult(score1, score2, expectationOneSet);
        }

        public static decimal ChanceOfHavingThisResult(int gameScore1, int gameScore2, decimal expectedToWinSet)
        {
            var numberOfSets = gameScore1 + gameScore2;

            var changeWinningGameWithSpecifiedResult = (double)Factorial(numberOfSets) / (double)(Factorial(gameScore1) * Factorial(gameScore2))
                                                       * Math.Pow((double)expectedToWinSet, gameScore1)
                                                       * Math.Pow((double)(1 - expectedToWinSet), gameScore2);

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

    public static class EloCalculator
    {
        public static decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var K = 50;

            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = gameScore1 > gameScore2 ? 1 : 0;

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var marginOfVicoryMultiplier = MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff);

            var outcome1 = (actualResult - expectedOutcome1);
            var player1Delta = K * outcome1 * marginOfVicoryMultiplier;

            return player1Delta;
        }

        public static decimal MarginOfVictoryMultiplier(int gameScore1, int gameScore2, decimal winnerEloDiff)
        {
            return (decimal)Math.Log(Math.Abs(gameScore1 - gameScore2) + 1) *
                   (2.2m / (winnerEloDiff * 0.001m + 2.2m));
        }

        public static decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            return ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);
        }

        private static decimal ExpectationOneSet(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            decimal n = 400;
            decimal x = oldRatingPlayer1 - oldRatingPlayer2;
            decimal exponent = -1 * (x / n);
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
}