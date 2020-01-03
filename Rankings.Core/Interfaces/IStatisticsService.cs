using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IStatisticsService
    {
        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
        IDictionary<Profile, EloStatsPlayer> TheNewRanking(string gameType, DateTime startDate, DateTime endDate);
        IEnumerable<char> History(string emailAddress);
        decimal WinPercentage(string emailAddress);
        decimal SetWinPercentage(string emailAddress);
        int RecordWinningStreak(string emailAddress);
        int CurrentWinningStreak(string emailAddress);
        decimal RecordEloStreak(string emailAddress);
        decimal CurrentEloStreak(string emailAddress);
    }
}