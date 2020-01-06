using System;
using System.Linq;
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
                var total1 = 0m;
                foreach (var x in Enumerable.Range(1, gameScore1))
                {
                    total1 += CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, 1, 0);
                }

                var total2 = 0m;
                foreach (var x in Enumerable.Range(1, gameScore2))
                {
                    total2 += CalculateDeltaPlayer(ratingPlayer2, ratingPlayer1, 1, 0);
                }

                var diff = (total1 - total2);
                setPoints = diff;// / (gameScore1 + gameScore2); // - total2 > 0 ? total1 - total2 : 0;
            }


            var expectedOutcome1 = GeneralEloCalculator.CalculateExpectationForBestOf(ratingPlayer1, ratingPlayer2, Math.Max(gameScore1, gameScore2));
            var actualResult = GeneralEloCalculator.ActualResult(gameScore1, gameScore2);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            //var sign = Math.Sign(delta);
            //var pi = (decimal)Math.PI/2;
            //delta = (decimal) Math.Atan((double) delta*5)/pi * kfactor;
            var outcome1 = (actualResult - expectedOutcome1);
            var marginOfVictoryMultiplier = 1;// GeneralEloCalculator.MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff);
            var resolveKFactorFor = ResolveKFactorFor(ratingPlayer1, gameScore1, gameScore2);
            var delta = outcome1
                        * (marginOfVictoryMultiplier)
                        * resolveKFactorFor;

            return delta;//+ setPoints;
        }

        private decimal ResolveKFactorFor(decimal rating, int gameScore1, int gameScore2)
        {
            var maxKfactor = ResolveMaxKFactor(gameScore1, gameScore2);
            return maxKfactor;
            var startElo = 1300;
            var targetElo = 1400;

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

            return 5;
        }
    }
}