﻿using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rankings.Core.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IGamesService _gamesService;
        private readonly EloConfiguration _eloConfiguration;
        private readonly ILogger<IStatisticsService> _logger;
        private readonly IEloCalculatorFactory _eloCalculatorFactory;

        public StatisticsService(IGamesService gamesService, EloConfiguration eloConfiguration, ILogger<IStatisticsService> logger, IEloCalculatorFactory eloCalculatorFactory)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _eloConfiguration = eloConfiguration ?? throw new ArgumentNullException(nameof(eloConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eloCalculatorFactory = eloCalculatorFactory ?? throw new ArgumentNullException(nameof(eloCalculatorFactory));
        }

        public IDictionary<Profile, EloStatsPlayer> Ranking(string gameType, DateTime startDate, DateTime endDate)
        {
            var eloStatsPlayers = EloStatsPlayers(gameType, startDate, endDate);
            return CalculateRanking(eloStatsPlayers, _eloConfiguration.NumberOfGames);
        }

        private Dictionary<Profile, EloStatsPlayer> CalculateRanking(Dictionary<Profile, EloStatsPlayer> eloStatsPlayers, int numberOfGames)
        {
            var rankedPlayers = eloStatsPlayers
                .Where(pair => pair.Value.NumberOfGames >= numberOfGames)
                .OrderByDescending(pair => pair.Value.EloScore).ThenBy(pair => pair.Key.DisplayName);

            var orderedRanking = new Dictionary<Profile, EloStatsPlayer>(rankedPlayers);

            var rankingCalculator = new RankingCalculator();
            foreach (var item in orderedRanking)
            {
                rankingCalculator.Push(item);
            }

            return rankingCalculator.Ranking;
        }

        public IEnumerable<char> History(string emailAddress, DateTime startDate, DateTime endDate)
        {
            return _gamesService
                .List(new GamesForPlayerInPeriodSpecification("tafeltennis", emailAddress, startDate, endDate))
                .TakeLast(7)
                .Select(game => new
                {
                    Score1 = string.Equals(game.Player1.EmailAddress, emailAddress,
                        StringComparison.CurrentCultureIgnoreCase)
                        ? game.Score1
                        : game.Score2,
                    Score2 = string.Equals(game.Player1.EmailAddress, emailAddress,
                        StringComparison.CurrentCultureIgnoreCase)
                        ? game.Score2
                        : game.Score1
                })
                .Select(g =>
                {
                    if (g.Score1 == g.Score2)
                        return 'D';

                    return g.Score1 > g.Score2 ? 'W' : 'L';
                });
        }

        public decimal WinPercentage(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var won = gamesByPlayer
                .Count(arg => arg.Score1 > arg.Score2);

            return (decimal) won / gamesByPlayer.Count;
        }

        public decimal SetWinPercentage(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var won = gamesByPlayer
                .Sum(arg => arg.Score1);

            var totalSets = gamesByPlayer.Sum(arg => arg.Score1 + arg.Score2);

            return (decimal) won / totalSets;
        }

        public int RecordWinningStreak(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Select(games => games.Count()).Max();
        }

        public Dictionary<Profile, int> RecordWinningStreak(DateTime startDate, DateTime endDate)
        {
            return StatsPerPlayer(startDate, endDate, RecordWinningStreak);
        }

        public Dictionary<Profile, decimal> FibonacciScore(DateTime startDate, DateTime endDate)
        {
            return StatsPerPlayer(startDate, endDate, FibonacciScore);
        }

        private decimal FibonacciScore(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var totals = gamesByPlayer.GroupBy(game => game.Player2).Select(games => new {Opponent = games.Key, Count = games.Count()}).ToDictionary(arg => arg.Opponent, arg => arg.Count);

            var fibonacciScore = 0m;
            while (true)
            {
                var numberOfPlayers = totals.Count;
                fibonacciScore += Fibonacci(numberOfPlayers);
                totals = totals.Select(pair => new {Player = pair.Key, Count = pair.Value - 1}).Where(arg => arg.Count > 0).ToDictionary(arg => arg.Player, arg => arg.Count);

                if (totals.Count == 0)
                    break;
            }

            //return (int) Math.Round(Math.Sqrt(fibonacciScore));
            return fibonacciScore;
        }

        private int Fibonacci(int n)
        {
            if (n >= 10)
                return 34 + (n-10);

            if (n == 1)
                return 0;

            if (n <= 3)
                return 1;

            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        public Dictionary<Profile, decimal> RecordEloStreak(DateTime startDate, DateTime endDate)
        {
            return StatsPerPlayer(startDate, endDate, RecordEloStreak);
        }

        private Dictionary<Profile, T> StatsPerPlayer<T>(DateTime startDate, DateTime endDate, Func<string, DateTime, DateTime, T> calc)
        {
            var players = _gamesService.List(new AllProfiles()).ToList();

            return players.ToDictionary(profile => profile, profile => calc(profile.EmailAddress, startDate, endDate));
        }

        public decimal GoatScore(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress, startDate, endDate);

            var total = 1200m;
            var goat = 1100m;
            foreach (var statGame in gamesByPlayer)
            {
                total += statGame.Delta1.Value;
                if (total > goat)
                    goat = total;
            }

            return goat;
        }

        public Dictionary<Profile, decimal> GoatScore(DateTime startDate, DateTime endDate)
        {
            return StatsPerPlayer(startDate, endDate, GoatScore);
        }

        public IEnumerable<IEnumerable<StatGame>> WinningStreaks(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress, startDate, endDate);

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();

            return series;
        }

        public IEnumerable<IEnumerable<StatGame>> LosingStreaks(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress, startDate, endDate);

            var series = gamesByPlayer.Split(game => game.Score1 < game.Score2).ToList();

            return series;
        }


        public int CurrentWinningStreak(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Last().Count();
        }

        public decimal RecordEloStreak(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Select(games => games.Sum(game => game.Delta1)).Max() ?? 0;
        }

        public decimal CurrentEloStreak(string emailAddress, DateTime startDate, DateTime endDate)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress, startDate, endDate);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Last().Sum(games => games.Delta1.Value);
        }

        private List<StatGame> EloGamesByPlayer(string emailAddress, DateTime startDate, DateTime endDate)
        {
            return EloGames("tafeltennis", startDate, endDate)
                .Where((game, i) =>
                    game.Game.Player1.EmailAddress.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase) ||
                    game.Game.Player2.EmailAddress.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase))
                .Select(game => string.Equals(game.Game.Player1.EmailAddress, emailAddress,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? new
                    {
                        Score1 = game.Game.Score1, Score2 = game.Game.Score2, Delta1 = game.Player1Delta,
                        Delta2 = game.Player2Delta
                    }
                    : new
                    {
                        Score1 = game.Game.Score2, Score2 = game.Game.Score1, Delta1 = game.Player2Delta,
                        Delta2 = game.Player1Delta
                    })
                .Select(arg => new StatGame(arg.Score1, arg.Score2, arg.Delta1, arg.Delta2)).ToList();
        }

        private List<StatGame> GamesByPlayer(string emailAddress, DateTime startDate, DateTime endDate)
        {
            // TODO tafeltennis hardcoded...
            return _gamesService
                .List(new GamesForPlayerInPeriodSpecification("tafeltennis", emailAddress, startDate, endDate))
                .Select(game =>
                    string.Equals(game.Player1.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase)
                        ? new {game.Score1, game.Score2, Player1 = game.Player1.EmailAddress, Player2 = game.Player2.EmailAddress}
                        : new {Score1 = game.Score2, Score2 = game.Score1, Player1 = game.Player2.EmailAddress, Player2 = game.Player1.EmailAddress})
                .Select(arg => new StatGame(arg.Score1, arg.Score2)
                {
                    Player1 = arg.Player1,
                    Player2 = arg.Player2
                })
                .ToList();
        }

        public IDictionary<Profile, decimal> GoatScores(string tafeltennis, in DateTime minValue, in DateTime maxValue)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Profile, EloStatsPlayer> EloStatsPlayers(string gameType, DateTime startDate, DateTime endDate)
        {
            var eloGames = EloGames(gameType, startDate, endDate).ToList();
            var allPlayers = _gamesService.List(new AllProfiles()).ToList();

            // All players have an initial elo score
            var eloStatsPlayers = allPlayers
                .ToDictionary(player => player,
                    player => new EloStatsPlayer
                    {
                        EloScore = _eloConfiguration.InitialElo, NumberOfGames = 0,
                        TimeNumberOne = new TimeSpan(0, 0, 0)
                    });

            if (!eloGames.Any())
                return eloStatsPlayers;

            var lastGameRegistrationDate = eloGames.First().Game.RegistrationDate;

            foreach (var eloGame in eloGames)
            {
                var player1 = eloStatsPlayers[eloGame.Game.Player1];
                var player2 = eloStatsPlayers[eloGame.Game.Player2];

                player1.EloScore += eloGame.Player1Delta;
                player1.NumberOfGames += 1;
                player2.EloScore += eloGame.Player2Delta;
                player2.NumberOfGames += 1;

                var ranking = CalculateRanking(eloStatsPlayers, _eloConfiguration.NumberOfGames);
                if (ranking.Any())
                {
                    var diff = eloGame.Game.RegistrationDate - lastGameRegistrationDate;

                    foreach (var player in ranking.Where(pair => pair.Value.Ranking == 1).Select(pair => pair.Key))
                    {
                        eloStatsPlayers[player].TimeNumberOne += diff;
                    }

                    // if (lastNumberOne != currentNumberOne)
                    // {
                    //     _logger.LogInformation( $" New nr1: {currentNumberOne.EmailAddress} {eloGame.Game.RegistrationDate} ==> {eloStatsPlayers[currentNumberOne].TimeNumberOne}");
                    //     if (lastNumberOne != null)
                    //      _logger.LogInformation( $" Old nr1: {lastNumberOne.EmailAddress} ==> {eloStatsPlayers[lastNumberOne].TimeNumberOne}");
                    // }
                }

                lastGameRegistrationDate = eloGame.Game.RegistrationDate;
            }

            var lastDate = DateTime.Now < endDate ? DateTime.Now : endDate;
            var diff2 = lastDate - lastGameRegistrationDate;
            foreach (var player in eloStatsPlayers.Where(pair => pair.Value.Ranking == 1).Select(pair => pair.Key))
            {
                eloStatsPlayers[player].TimeNumberOne += diff2;
            }

            return eloStatsPlayers;
        }

        public IEnumerable<EloGame> EloGames(string gameType, DateTime startDate, DateTime endDate)
        {
            // All players should be in the ranking. Not restrictions (not yet :-))
            var allPlayers = _gamesService.List(new AllProfiles()).ToList();

            // All players have an initial elo score
            var ranking = allPlayers
                .ToDictionary(player => player, player => new EloStatsPlayer {EloScore = _eloConfiguration.InitialElo, NumberOfGames = 0});

            // Now calculate current elo score based on all games played
            var games = _gamesService.List(new GamesForPeriodSpecification(gameType, startDate, endDate)).ToList();
            foreach (var game in games.OrderBy(game => game.RegistrationDate))
            {
                var eloCalculator = _eloCalculatorFactory.Create(game.RegistrationDate.Year);

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ranking[game.Player1];
                var oldRatingPlayer2 = ranking[game.Player2];

                var player1Delta = eloCalculator.CalculateDeltaPlayer(oldRatingPlayer1.EloScore, oldRatingPlayer2.EloScore, game.Score1, game.Score2);
                var player2Delta = eloCalculator.CalculateDeltaPlayer(oldRatingPlayer2.EloScore, oldRatingPlayer1.EloScore, game.Score2, game.Score1);

                var eloGames = new EloGame(game, ranking[game.Player1].EloScore, ranking[game.Player2].EloScore, player1Delta, player2Delta);

                ranking[game.Player1].EloScore = oldRatingPlayer1.EloScore + player1Delta;
                ranking[game.Player2].EloScore = oldRatingPlayer2.EloScore + player2Delta;

                yield return eloGames;
            }
        }
    }

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