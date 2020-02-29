using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Models;

namespace Rankings.Core.Services
{
    internal class RankingCalculator
    {
        public Dictionary<Profile, EloStatsPlayer> Ranking { get; }

        public RankingCalculator()
        {
            Ranking = new Dictionary<Profile, EloStatsPlayer>();
        }

        public void Push(KeyValuePair<Profile, EloStatsPlayer> item)
        {
            var lastPlayer = Ranking.LastOrDefault();
            Ranking.Add(item.Key, item.Value);

            if (lastPlayer.Key == null)
            {
                item.Value.Ranking = 1;
                return;
            }

            if (lastPlayer.Value.EloScore == item.Value.EloScore)
            {
                item.Value.Ranking = lastPlayer.Value.Ranking;
                return;
            }

            item.Value.Ranking = Ranking.Count;
        }
    }
}