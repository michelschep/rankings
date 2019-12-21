using System;
using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services.ToBeObsolete;

namespace Rankings.Core.Services
{
    public class RankingStats
    {
        public Ranking Ranking { get; set; }
        public List<Game> Games { get; set; }
        public DateTime DateTime { get; set; }
        public IDictionary<Profile, NewPlayerStats> NewPlayerStats { get; set; }
    }
}