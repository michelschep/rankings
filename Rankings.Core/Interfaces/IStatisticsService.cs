using System;
using System.Collections.Generic;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IStatisticsService: IRankingStats
    {
        KeyValuePair<DateTime, RankingStats> CalculateStats();
    }
}