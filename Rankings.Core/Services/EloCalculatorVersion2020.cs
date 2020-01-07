using System;
using Rankings.Core.Services.ToBeObsolete;

namespace Rankings.Core.Services
{
    public class EloCalculatorVersion2020 : IEloCalculator
    {
        public decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var setPoints = 0m;
            if (gameScore1 + gameScore2 > 1)
            {
                var total1 = gameScore1 * CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, 1, 0);
                var total2 = gameScore2 * CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, 0, 1);

                var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

                setPoints = (total1+total2) * GeneralEloCalculator.MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff); 
            }

            var expectedOutcome1 = GeneralEloCalculator.CalculateExpectationForBestOf(ratingPlayer1, ratingPlayer2, Math.Max(gameScore1, gameScore2));
            var actualResult = GeneralEloCalculator.ActualResult(gameScore1, gameScore2);

            var outcome1 = (actualResult - expectedOutcome1);
            var resolveKFactorFor = ResolveKFactorFor(ratingPlayer1, gameScore1, gameScore2);

            return ((75 * outcome1 * resolveKFactorFor) + (25 * setPoints))/100;
        }

        private decimal ResolveKFactorFor(decimal rating, int gameScore1, int gameScore2)
        {
            var maxKfactor = ResolveMaxKFactor(gameScore1, gameScore2);

            var startElo = 1300;
            var targetElo = 1800;

            if (rating <= startElo)
                return maxKfactor;

            var factor = rating > targetElo ? maxKfactor / 2m : maxKfactor - ((rating - startElo) / (targetElo - startElo)) * (maxKfactor / 2m);
            var result = factor;

            return result;
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
    }
}