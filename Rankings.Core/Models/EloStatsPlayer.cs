using System;

namespace Rankings.Core.Models
{
    public class EloStatsPlayer
    {
        public int Ranking { get; set; }
        public decimal EloScore { get; set; }
        public int NumberOfGames { get; set; }
        public TimeSpan TimeNumberOne { get; set; }
    }
}