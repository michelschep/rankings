using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;

namespace Rankings.Core.Services.ToBeObsolete
{
    public class ObsoleteRanking
    {
        private readonly Dictionary<Profile, ObsoletePlayerStats> _ratings;

        public ObsoleteRanking(Dictionary<Profile, ObsoletePlayerStats> ratings)
        {
            _ratings = ratings ?? throw new ArgumentNullException(nameof(ratings));
        }

        public Dictionary<Profile, ObsoletePlayerStats> DeprecatedRatings => ConvertRatings(_ratings);

        private Dictionary<Profile, ObsoletePlayerStats> ConvertRatings(Dictionary<Profile, ObsoletePlayerStats> ratings)
        {
            var convertedRatings = new Dictionary<Profile, ObsoletePlayerStats>();
            foreach (var stats in ratings)
            {
                convertedRatings.Add(stats.Key, ConvertStats(stats.Value));
            }

            return convertedRatings;
        }

        public ObsoletePlayerStats ForPlayer(string emailAddress)
        {
            return _ratings.Where(pair => string.Equals(pair.Key.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase))
                .Select(pair => ConvertStats(pair.Value))
                .Single();
        }

        public IEnumerable<ObsoletePlayerStats> PlayerStats()
        {
            return _ratings.Values.Select(ConvertStats);
        }

        private static ObsoletePlayerStats ConvertStats(ObsoletePlayerStats stats)
        {
            // TODO get rid of or at least with mapper
            return new ObsoletePlayerStats
            {
                Ranking = stats.Ranking,
                NumberOfSets = stats.NumberOfSets,
                History = stats.History,
                NumberOfSetWins = stats.NumberOfSetWins,
                NumberOfWins = stats.NumberOfWins,
                NumberOfGames = stats.NumberOfGames,
                // TODO fix 0.001 hack
                WinPercentage = Math.Round((100m*stats.NumberOfWins/(stats.NumberOfGames+0.001m)), 2, MidpointRounding.AwayFromZero),
                SetWinPercentage = Math.Round((100m*stats.NumberOfSetWins/(stats.NumberOfSets+0.001m)), 2, MidpointRounding.AwayFromZero),
                BestEloSeries = stats.BestEloSeries,
                CurrentEloSeries = stats.CurrentEloSeries,
                Goat =  stats.Goat,
                TimeNumberOne = stats.TimeNumberOne
            };
        }
    }
}