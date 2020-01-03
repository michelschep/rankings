using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IStatisticsService
    {
        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
        IDictionary<Profile, EloStatsPlayer> Ranking(string gameType, DateTime startDate, DateTime endDate);
        IEnumerable<char> History(string emailAddress, DateTime startDate, DateTime endDate);
        decimal WinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        decimal SetWinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        int RecordWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        int CurrentWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal RecordEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal CurrentEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
    }
}