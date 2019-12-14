using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;

namespace Rankings.Core.Services.ToBeObsolete
{
    [Obsolete("Well, you still need this. We want to get rid of it!")]
    public class Ranking
    {
        private readonly Dictionary<Profile, PlayerStats> _ratings;

        public Ranking(Dictionary<Profile, PlayerStats> ratings)
        {
            _ratings = ratings ?? throw new ArgumentNullException(nameof(ratings));
        }

        public Dictionary<Profile, PlayerStats> DeprecatedRatings => ConvertRatings(_ratings);

        private Dictionary<Profile, PlayerStats> ConvertRatings(Dictionary<Profile, PlayerStats> ratings)
        {
            var convertedRatings = new Dictionary<Profile, PlayerStats>();
            foreach (var stats in ratings)
            {
                convertedRatings.Add(stats.Key, ConvertStats(stats.Value));
            }

            return convertedRatings;
        }

        public PlayerStats ForPlayer(string emailAddress)
        {
            return _ratings.Where(pair => string.Equals(pair.Key.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase))
                .Select(pair => ConvertStats(pair.Value))
                .Single();
        }

        public IEnumerable<PlayerStats> PlayerStats()
        {
            return _ratings.Values.Select(ConvertStats);
        }

        private static PlayerStats ConvertStats(PlayerStats stats)
        {
            // TODO get rid of or at least with mapper
            return new PlayerStats
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