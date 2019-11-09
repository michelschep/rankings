﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;

namespace Rankings.Core.Services
{
    public class Ranking
    {
        private readonly Dictionary<Profile, PlayerStats> _ratings;
        private readonly int _precision;

        public Ranking(Dictionary<Profile, PlayerStats> ratings, int precision)
        {
            _ratings = ratings ?? throw new ArgumentNullException(nameof(ratings));
            _precision = precision;
        }

        public Dictionary<Profile, PlayerStats> DeprecatedRatings
        {
            get { return ConvertRatings(_ratings); }
        }

        private Dictionary<Profile, PlayerStats> ConvertRatings(Dictionary<Profile, PlayerStats> ratings)
        {
            var convertedRatings = new Dictionary<Profile, PlayerStats>();
            foreach (var stats in ratings)
            {
                convertedRatings.Add(stats.Key, ConvertStats(stats.Value, _precision));
            }

            return convertedRatings;
        }

        public PlayerStats ForPlayer(string emailAddress)
        {
            return _ratings.Where(pair => string.Equals(pair.Key.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase))
                .Select(pair => ConvertStats(pair.Value, _precision))
                .Single();
        }

        public IEnumerable<PlayerStats> PlayerStats()
        {
            return _ratings.Values.Select(ConvertStats);
        }

        private static PlayerStats ConvertStats(PlayerStats stats, int precision)
        {
            return new PlayerStats
            {
                Ranking = Math.Round(stats.Ranking, precision, MidpointRounding.AwayFromZero),
                NumberOfSets = stats.NumberOfSets,
                History = stats.History,
                NumberOfSetWins = stats.NumberOfSetWins,
                NumberOfWins = stats.NumberOfWins,
                NumberOfGames = stats.NumberOfGames,
                // TODO fix 0.001 hack
                WinPercentage = Math.Round((100m*stats.NumberOfWins/(stats.NumberOfGames+0.001m)), 2, MidpointRounding.AwayFromZero),
                SetWinPercentage = Math.Round((100m*stats.NumberOfSetWins/(stats.NumberOfSets+0.001m)), 2, MidpointRounding.AwayFromZero),
            };
        }
    }
}