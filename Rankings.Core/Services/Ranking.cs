using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;

namespace Rankings.Core.Services
{
    public class Ranking
    {
        private readonly Dictionary<Profile, PlayerStats> _ratings;

        public Ranking(Dictionary<Profile, PlayerStats> ratings)
        {
            _ratings = ratings ?? throw new ArgumentNullException(nameof(ratings));
        }

        public Dictionary<Profile, PlayerStats> OldRatings => _ratings;

        public PlayerStats ForPlayer(string emailAddress)
        {
            return _ratings.Where(pair => string.Equals(pair.Key.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase)).Select(pair => ConvertStats(pair.Value)).Single();
        }

        public IEnumerable<PlayerStats> PlayerStats()
        {
            return _ratings.Values.Select(ConvertStats);
        }

        private static PlayerStats ConvertStats(PlayerStats stats)
        {
            return new PlayerStats
            {
                Ranking = Math.Round(stats.Ranking, 0, MidpointRounding.AwayFromZero),
                NumberOfSets = stats.NumberOfSets,
                History = stats.History,
                NumberOfSetWins = stats.NumberOfSetWins,
                NumberOfWins = stats.NumberOfWins,
                NumberOfGames = stats.NumberOfGames
            };
        }
    }
}