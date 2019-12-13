using System;

namespace Rankings.Core.Services
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal d, int precision = 0)
        {
            return Math.Round(d, precision, MidpointRounding.AwayFromZero);
        }
    }
    public class EloCalculator
    {
        private readonly EloConfiguration _eloConfiguration;
        
        private static decimal _n = 400;
        private static decimal _k = 50; 
        private readonly bool _withMarginOfVictory;

        public EloCalculator(EloConfiguration eloConfiguration): this(eloConfiguration.N, eloConfiguration.Kfactor, eloConfiguration.WithMarginOfVictory)
        {
            _eloConfiguration = eloConfiguration;
        }

        [Obsolete]
        public EloCalculator(decimal n, decimal kfactor, bool withMarginOfVictory = true)
        {
            _n = n;
            _k = kfactor;
            _withMarginOfVictory = withMarginOfVictory;
        }

        public decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = ActualResult(gameScore1, gameScore2);

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var marginOfVictoryMultiplier = _withMarginOfVictory ? MarginOfVictoryMultiplier(gameScore1, gameScore2, winnerEloDiff) : 1;

            var outcome1 = (actualResult - expectedOutcome1);

            // TODO next version elo. Less influence margin of victory. And a smaller k factor (25?)
            //var player1Delta = _k * outcome1 * (decimal)Math.Sqrt(Math.Sqrt((double)marginOfVictoryMultiplier));
            var player1Delta = _k * outcome1 * marginOfVictoryMultiplier;

            return player1Delta;
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
            decimal exponent = (ratingPlayer2 - ratingPlayer1) / _n;
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
}