using System;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IRankingStats
    {
        Ranking Ranking(string gameType, DateTime rankingDate);
        Ranking Ranking(string gameType);
        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
    }
}