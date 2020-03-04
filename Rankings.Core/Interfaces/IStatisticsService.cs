using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Models;
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
        Dictionary<Profile, decimal> StrengthGamesPerPlayer(in DateTime startDate, in DateTime endDate);
        Dictionary<Profile, decimal> StrengthOponentPerPlayer(in DateTime startDate, in DateTime endDate);
        Dictionary<Profile, decimal> StrengthWinsPerPlayer(in DateTime startDate, in DateTime endDate);
        Dictionary<Profile, decimal> StrengthLostsPerPlayer(in DateTime startDate, in DateTime endDate);

        Dictionary<Profile, Dictionary<string, decimal>> TotalElo(string gameType, DateTime startDate, DateTime endDate);

        IEnumerable<char> History(string emailAddress, DateTime startDate, DateTime endDate);
        decimal WinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        decimal SetWinPercentage(string emailAddress, DateTime startDate, DateTime endDate);
        int RecordWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal GoatScore(string emailAddress, DateTime startDate, DateTime endDate);
        int CurrentWinningStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal RecordEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
        decimal CurrentEloStreak(string emailAddress, DateTime startDate, DateTime endDate);
        IEnumerable<GameSummary> GameSummaries(in DateTime startDate, in DateTime endDate);
        IEnumerable<StatGame> EloGames(string emailAddress);
        IEnumerable<Streak> WinningStreaksPlayer(Profile profile, DateTime startDate, DateTime endDate);
        IEnumerable<Streak> WinningStreaks(DateTime startDate, DateTime endDate);
        IEnumerable<Streak> LosingStreaksPlayer(Profile profile, DateTime startDate, DateTime endDate);
        IEnumerable<Streak> LosingStreaks(DateTime startDate, DateTime endDate);
    }

    public class GameSummary
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public int TotalGames { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int PercentageScore1 { get; set; }
        public int PercentageScore2 { get; set; }
        public int Set1 { get; set; }
        public int Set2 { get; set; }
        public int PercentageSet1 { get; set; }
        public int PercentageSet2 { get; set; }
    }
}