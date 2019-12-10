﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;

namespace Rankings.Core.Services
{
    public class EloConfiguration
    {
        public EloConfiguration(int kfactor, int n, bool withMarginOfVictory, int initialElo)
        {
            Kfactor = kfactor;
            N = n;
            WithMarginOfVictory = withMarginOfVictory;
            InitialElo = initialElo;
        }

        public int Kfactor { get; }
        public int N { get; }
        public bool WithMarginOfVictory { get; }
        public int InitialElo { get; }
    }

    public class StatisticsService : IStatisticsService
    {
        private readonly IGamesService _gamesService;
        private readonly EloConfiguration _eloConfiguration;
        private readonly EloCalculator _eloCalculator;

        public StatisticsService(IGamesService gamesService, EloConfiguration eloConfiguration)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _eloConfiguration = eloConfiguration;
            _eloCalculator = new EloCalculator(eloConfiguration);
        }

        public KeyValuePair<DateTime, RankingStats> CalculateStats(DateTime startDate, DateTime endDate)
        {
            var allGames = _gamesService.List(new GamesForPeriodSpecification("tafeltennis", startDate, endDate)).ToList();
            var dateTimes = allGames.Select(g => g.RegistrationDate).ToList();
            var now = DateTime.Now;
            dateTimes.Add(now);

            var history = new Dictionary<DateTime, RankingStats>();
            foreach (var game in allGames)
            {
                if (!history.ContainsKey(game.RegistrationDate))
                    history.Add(game.RegistrationDate, new RankingStats());

                var rankingStats = history[game.RegistrationDate];
                rankingStats.DateTime = game.RegistrationDate;
                rankingStats.Games = allGames.Where(g => g.RegistrationDate == game.RegistrationDate).ToList();
            }

            history.Add(now, new RankingStats
            {
                DateTime = now,
                Games = new List<Game>(),
            });

            foreach (var pointInTime in history)
            {
                var ranking = Ranking("tafeltennis", pointInTime.Key);
                pointInTime.Value.Ranking = ranking;

                // Create stats for all players
                pointInTime.Value.NewPlayerStats = new Dictionary<Profile, NewPlayerStats>();
            }

            //Profile previousNumberOneProfile = null;
            //PlayerStats previousNumberOneStats = null;
            DateTime? previousPointInTimeDate = null;
            RankingStats previousPointInTimeRankingStats = null;

            foreach (var pointInTime in history)
            {
                // Init data player stats
                InitStats(previousPointInTimeDate, pointInTime, previousPointInTimeRankingStats, _eloConfiguration.InitialElo);

                // game stats
                CalculateGameStats(pointInTime.Value, previousPointInTimeRankingStats);

                // Who is number one?
                var ranking = pointInTime.Value.Ranking.DeprecatedRatings.OrderByDescending(pair => pair.Value.Ranking).ToList();
                foreach (var item in ranking)
                {
                    var player = pointInTime.Value.NewPlayerStats[item.Key];
                    player.Rating = (int)item.Value.Ranking;

                    if (player.Rating > player.GoatRating)
                        player.GoatRating = player.Rating;
                }

                var numberOne = ranking.First().Key;
                if (previousPointInTimeDate != null)
                {
                    pointInTime.Value.NewPlayerStats[numberOne].NumberOfGamesNumberOne += 1;

                    var previousTimeNumberOne = previousPointInTimeRankingStats.NewPlayerStats[numberOne].TimeNumberOne;
                    pointInTime.Value.NewPlayerStats[numberOne].TimeNumberOne = previousTimeNumberOne +
                                                                                (int)(pointInTime.Key -
                                                                                      previousPointInTimeDate.Value)
                                                                                .TotalMinutes;
                }

                previousPointInTimeDate = pointInTime.Key;
                previousPointInTimeRankingStats = pointInTime.Value;
            }

            var lastPointInTime = history.OrderByDescending(h => h.Key).First();

            return lastPointInTime;

        }

        public Ranking Ranking(string gameType)
        {
            return Ranking(gameType, DateTime.MinValue, DateTime.MaxValue);
        }

        public Ranking Ranking(string gameType, DateTime endDate)
        {
            return Ranking(gameType, DateTime.MinValue, endDate);
        }

        public Dictionary<Profile, decimal> EloNew(string gameType, DateTime startDate, DateTime endDate)
        {
            // All players should be in the ranking. Not restrictions (not yet :-))
            var allPlayers = _gamesService.List<Profile>(new AllProfiles());

            // All players have an initial elo score
            var ranking = new Dictionary<Profile, decimal>();
            foreach (var player in allPlayers)
            {
                ranking.Add(player, _eloConfiguration.InitialElo);
            }

            // Now calculate current elo score based on all games played
            var games = _gamesService.List(new GamesForPeriodSpecification(gameType, startDate, endDate)).ToList();
            foreach (var game in games.OrderBy(game => game.RegistrationDate))
            {
                // TODO for tafeltennis a 0-0 is not a valid result. For time related games it is possible
                // For now ignore a 0-0
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ranking[game.Player1];
                var oldRatingPlayer2 = ranking[game.Player2];

                var player1Delta = CalculateDeltaFirstPlayer(oldRatingPlayer1, oldRatingPlayer2, game.Score1, game.Score2);

                ranking[game.Player1] = oldRatingPlayer1 + player1Delta;
                ranking[game.Player2] = oldRatingPlayer2 - player1Delta;
            }

            return ranking;
        }

        public Ranking Ranking(string gameType, DateTime startDate, DateTime endDate)
        {
            var gamesSpecification = new GamesForPeriodSpecification(gameType, startDate, endDate);
            var games = _gamesService.List(gamesSpecification).ToList();

            var ratings = new Dictionary<Profile, PlayerStats>();
            foreach (var profile in games.SelectMany(game => new List<Profile> { game.Player1, game.Player2 }).Distinct())
            {
                ratings.Add(profile, new PlayerStats
                {
                    NumberOfGames = 0,
                    NumberOfSetWins = 0,
                    NumberOfSets = 0,
                    NumberOfWins = 0,
                    Ranking = _eloConfiguration.InitialElo,
                    History = "",
                    CurrentEloSeries = 0,
                    BestEloSeries = 0,
                    SkalpStreak = 0,
                    Goat = 0
                });
            }

            foreach (var game in games.OrderBy(game => game.RegistrationDate))
            {
                // TODO for tafeltennis a 0-0 is not a valid result. For time related games it is possible
                // For now ignore a 0-0
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];

                var player1Delta = CalculateDeltaFirstPlayer(oldRatingPlayer1.Ranking, oldRatingPlayer2.Ranking, game.Score1, game.Score2);

                var newRatingPlayer1 = oldRatingPlayer1.Ranking + player1Delta;
                var newRatingPlayer2 = oldRatingPlayer2.Ranking - player1Delta;

                if (player1Delta > 0)
                {
                    ratings[game.Player1].CurrentEloSeries += player1Delta;
                    ratings[game.Player2].CurrentEloSeries = 0;

                    if (ratings[game.Player1].CurrentEloSeries > ratings[game.Player1].BestEloSeries)
                    {
                        ratings[game.Player1].BestEloSeries = ratings[game.Player1].CurrentEloSeries;
                    }
                }

                if (player1Delta < 0)
                {
                    ratings[game.Player2].CurrentEloSeries += -1 * player1Delta;
                    ratings[game.Player1].CurrentEloSeries = 0;

                    if (ratings[game.Player2].CurrentEloSeries > ratings[game.Player2].BestEloSeries)
                    {
                        ratings[game.Player2].BestEloSeries = ratings[game.Player2].CurrentEloSeries;
                    }
                }

                ratings[game.Player1].Ranking = newRatingPlayer1;
                ratings[game.Player2].Ranking = newRatingPlayer2;

                ratings[game.Player1].NumberOfGames += 1;
                ratings[game.Player2].NumberOfGames += 1;

                ratings[game.Player1].NumberOfWins += game.Score1 > game.Score2 ? 1 : 0;
                ratings[game.Player2].NumberOfWins += game.Score2 > game.Score1 ? 1 : 0;

                if (game.Score1 > game.Score2)
                {
                    ratings[game.Player1].History += "W";
                    ratings[game.Player2].History += "L";
                }

                if (game.Score1 < game.Score2)
                {
                    ratings[game.Player1].History += "L";
                    ratings[game.Player2].History += "W";
                }

                if (game.Score1 == game.Score2)
                {
                    ratings[game.Player1].History += "D";
                    ratings[game.Player2].History += "D";
                }

                ratings[game.Player1].NumberOfSets += game.Score1 + game.Score2;
                ratings[game.Player2].NumberOfSets += game.Score1 + game.Score2;

                ratings[game.Player1].NumberOfSetWins += game.Score1;
                ratings[game.Player2].NumberOfSetWins += game.Score2;

                // goat
                if (ratings[game.Player1].Ranking > ratings[game.Player1].Goat)
                    ratings[game.Player1].Goat = ratings[game.Player1].Ranking;
                if (ratings[game.Player2].Ranking > ratings[game.Player2].Goat)
                    ratings[game.Player2].Goat = ratings[game.Player2].Ranking;

            }

            return new Ranking(ratings);
        }

        public decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            return _eloCalculator.CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);
        }

        private void InitStats(DateTime? previousPointInTimeDate,
            KeyValuePair<DateTime, RankingStats> pointInTime, RankingStats previousPointInTimeRankingStats,
            int initialElo)
        {
            var deprecatedRatingsKeys = Ranking("tafeltennis").DeprecatedRatings.Keys;
            foreach (var profile in deprecatedRatingsKeys)
            {
                if (previousPointInTimeDate == null)
                {
                    pointInTime.Value.NewPlayerStats.Add(profile, new NewPlayerStats
                    {
                        Profile = profile,
                        Rating = 0,
                        GoatRating = 0,
                        TimeNumberOne = 0,
                        NumberOfGamesNumberOne = 0,
                        NumberOfCleanSheets = 0,
                        NumberOfGames = 0,
                        EloWonOpponent = 0,
                        EloLostOpponent = 0,
                        Won = 0,
                        WonStreak = 0,
                        WonStreakRecord = 0,
                        Lost = 0,
                    });
                }
                else
                {
                    var previousStats = previousPointInTimeRankingStats.NewPlayerStats[profile];
                    pointInTime.Value.NewPlayerStats.Add(profile, new NewPlayerStats
                    {
                        Profile = profile,
                        Rating = previousStats.Rating,
                        GoatRating = previousStats.GoatRating,
                        TimeNumberOne = previousStats.TimeNumberOne,
                        NumberOfGamesNumberOne = previousStats.NumberOfGamesNumberOne,
                        NumberOfCleanSheets = previousStats.NumberOfCleanSheets,
                        NumberOfGames = previousStats.NumberOfGames,
                        EloWonOpponent = previousStats.EloWonOpponent,
                        EloLostOpponent = previousStats.EloLostOpponent,
                        Won = previousStats.Won,
                        WonStreak = previousStats.WonStreak,
                        WonStreakRecord = previousStats.WonStreakRecord,
                        Lost = previousStats.Lost,
                    });
                }
            }
        }

        private static void CalculateGameStats(RankingStats now, RankingStats previous)
        {
            foreach (var game in now.Games)
            {
                var statsPlayer1 = now.NewPlayerStats[game.Player1];
                var statsPlayer2 = now.NewPlayerStats[game.Player2];
                statsPlayer1.NumberOfGames += 1;
                statsPlayer2.NumberOfGames += 1;

                if (game.Score1 == 0)
                {
                    statsPlayer2.NumberOfCleanSheets += 1;
                }

                if (game.Score2 == 0)
                {
                    statsPlayer1.NumberOfCleanSheets += 1;
                }

                if (game.Score1 > game.Score2)
                {
                    statsPlayer1.Won += 1;
                    statsPlayer1.WonStreak += 1;
                    if (statsPlayer1.WonStreak > statsPlayer1.WonStreakRecord)
                        statsPlayer1.WonStreakRecord = statsPlayer1.WonStreak;

                    statsPlayer2.Lost += 1;
                    statsPlayer2.WonStreak = 0;

                    statsPlayer1.EloWonOpponent += previous?.NewPlayerStats[game.Player2].Rating ?? 1200;
                    statsPlayer2.EloLostOpponent += previous?.NewPlayerStats[game.Player1].Rating ?? 1200;
                }

                if (game.Score2 > game.Score1)
                {
                    statsPlayer2.Won += 1;
                    statsPlayer2.WonStreak += 1;
                    if (statsPlayer2.WonStreak > statsPlayer2.WonStreakRecord)
                        statsPlayer2.WonStreakRecord = statsPlayer2.WonStreak;

                    statsPlayer1.Lost += 1;
                    statsPlayer1.WonStreak = 0;

                    statsPlayer2.EloWonOpponent += previous?.NewPlayerStats[game.Player1].Rating ?? 1200;
                    statsPlayer1.EloLostOpponent += previous?.NewPlayerStats[game.Player2].Rating ?? 1200;
                }
            }
        }
    }
}