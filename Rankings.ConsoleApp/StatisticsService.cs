using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Services;

namespace Rankings.ConsoleApp
{
    public class StatisticsService
    {
        private readonly RankingService _rankingService;

        public StatisticsService(RankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public KeyValuePair<DateTime, RankingStats> CalculateStats()
        {
            DateTime currentDate = DateTime.MinValue;

            var days = new Dictionary<string, int>();
            var games = new Dictionary<string, int>();
            var minutes = new Dictionary<string, double>();

            _rankingService.GameTypes();
            var allGames = _rankingService.Games().Where(g => g.GameType.Code == "tafeltennis").ToList();
            var dateTimes = allGames.Select(g => g.RegistrationDate).ToList();
            var Now = DateTime.Now;
            dateTimes.Add(Now);
            DateTime previousGame = allGames.First().RegistrationDate;

            var history = new Dictionary<DateTime, RankingStats>();
            foreach (var game in allGames)
            {
                if (!history.ContainsKey(game.RegistrationDate))
                    history.Add(game.RegistrationDate, new RankingStats());

                var rankingStats = history[game.RegistrationDate];
                rankingStats.DateTime = game.RegistrationDate;
                rankingStats.Games = allGames.Where(g => g.RegistrationDate == game.RegistrationDate).ToList();
            }

            history.Add(Now, new RankingStats
            {
                DateTime = Now,
                Games = new List<Game>(),
            });

            foreach (var pointInTime in history)
            {
                var ranking = _rankingService.Ranking("tafeltennis", pointInTime.Key);
                pointInTime.Value.Ranking = ranking;

                // Create stats for all players
                pointInTime.Value.NewPlayerStats = new Dictionary<Profile, NewPlayerStats>();
            }

            Profile previousNumberOneProfile = null;
            PlayerStats previousNumberOneStats = null;
            DateTime? previousPointInTimeDate = null;
            RankingStats previousPointInTimeRankingStats = null;

            foreach (var pointInTime in history)
            {
                // Init data player stats
                InitStats(_rankingService, previousPointInTimeDate, pointInTime, previousPointInTimeRankingStats);

                // game stats
                CalculateGameStats(pointInTime.Value, previousPointInTimeRankingStats);

                // Who is number one?
                var ranking = pointInTime.Value.Ranking.DeprecatedRatings.OrderByDescending(pair => pair.Value.Ranking);
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


        private static void InitStats(RankingService rankingService, DateTime? previousPointInTimeDate,
            KeyValuePair<DateTime, RankingStats> pointInTime, RankingStats previousPointInTimeRankingStats)
        {
            var deprecatedRatingsKeys = rankingService.Ranking("tafeltennis").DeprecatedRatings.Keys;
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
                        Lost = 0
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
                        Lost = previousStats.Lost
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