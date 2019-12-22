using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;

namespace Rankings.Core.Interfaces
{
    public interface IStatisticsService
    {
        [Obsolete("Well, you still need this. We want to get rid of it!")]
        KeyValuePair<DateTime, RankingStats> CalculateStats(DateTime startDate, DateTime endDate);

        [Obsolete("Well, you still need this. We want to get rid of it!")]
        ObsoleteRanking Ranking(string gameType, DateTime startDate, DateTime endDate);

        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
        Dictionary<Profile, decimal> EloStats(string gameType, DateTime startDate, DateTime endDate);
    }
}