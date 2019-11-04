using System;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IRankingStats
    {
        Ranking Ranking(string gameType);
        Ranking Ranking(string gameType, DateTime endDate);
        Ranking Ranking(string gameType, DateTime startDate, DateTime endDate);
        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
    }
}