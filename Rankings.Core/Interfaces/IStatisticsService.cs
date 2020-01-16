using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IStatisticsService
    {
        IDictionary<Profile, EloStatsPlayer> Ranking(string gameType, DateTime startDate, DateTime endDate);
        Dictionary<Profile, int> RecordWinningStreak(DateTime startDate, DateTime endDate);
        Dictionary<Profile, decimal> FibonacciScore(DateTime startDate, DateTime endDate);
        Dictionary<Profile, decimal> RecordEloStreak(DateTime startDate, DateTime endDate);
        Dictionary<Profile, decimal> GoatScore(DateTime startDate, DateTime endDate);

        IEnumerable<char> History(string emailAddress, DateTime startDate, DateTime endDate);
        decimal WinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        decimal SetWinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        int RecordWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal GoatScore(string emailAddress, DateTime startDate, DateTime endDate);
        int CurrentWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal RecordEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal CurrentEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
    }
}