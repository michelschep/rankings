using System;
using System.Collections.Generic;
using Rankings.Core.Entities;

namespace Rankings.Core.Services.ToBeObsolete
{
    public class RankingStats
    {
        public ObsoleteRanking ObsoleteRanking { get; set; }
        public List<Game> Games { get; set; }
        public DateTime DateTime { get; set; }
        public IDictionary<Profile, ObsoleteNewPlayerStats> NewPlayerStats { get; set; }
    }
}