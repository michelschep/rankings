using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rankings.Core.Services
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> items, Predicate<T> p)
        {
            while (true)
            {
                items = items.SkipWhile(i => !p(i));
                var trueItems = items.TakeWhile(i => p(i)).ToList();
                if (trueItems.Count > 0)
                {
                    yield return trueItems;
                    items = items.Skip(trueItems.Count);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public class NewStatisticsService : IStatisticsService
    {
        private readonly IGamesService _gamesService; 
        private readonly EloConfiguration _eloConfiguration; 
        private readonly ILogger<IStatisticsService> _logger;
        private readonly EloCalculator _eloCalculator;
        private readonly OldStatisticsService _oldRankingOldStatisticsService;

        public NewStatisticsService(IGamesService gamesService, EloConfiguration eloConfiguration, ILogger<IStatisticsService> logger, EloCalculator eloCalculator, OldStatisticsService oldRankingOldStatisticsService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _eloConfiguration = eloConfiguration;
            _eloCalculator = eloCalculator ?? throw new ArgumentNullException(nameof(eloCalculator));
            _oldRankingOldStatisticsService = oldRankingOldStatisticsService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDictionary<Profile, EloStatsPlayer> TheNewRanking(string gameType, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Calculate ELO new");

            var eloStatsPlayers = EloStatsPlayers(gameType, startDate, endDate);
            var orderedRanking = CalculateRanking(eloStatsPlayers, _eloConfiguration.NumberOfGames);

            return orderedRanking;
        }

        private Dictionary<Profile, EloStatsPlayer> CalculateRanking(Dictionary<Profile, EloStatsPlayer> eloStatsPlayers, int numberOfGames)
        {
            var rankedPlayers = eloStatsPlayers
                .Where(pair => pair.Value.NumberOfGames >= numberOfGames)
                .OrderByDescending(pair => pair.Value.EloScore).ThenBy(pair => pair.Key.DisplayName);

            var orderedRanking = new Dictionary<Profile, EloStatsPlayer>(rankedPlayers);

            var index = 1;
            foreach (var item in orderedRanking.Values)
            {
                item.Ranking = index++;
            }

            return orderedRanking;
        }

        public IEnumerable<char> History(string emailAddress)
        {
            return _gamesService
                .List(new GamesForPlayerInPeriodSpecification("tafeltennis", emailAddress, DateTime.MinValue, DateTime.MaxValue))
                .TakeLast(7)
                .Select(game => new { 
                    Score1 = string.Equals(game.Player1.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase) ? game.Score1 : game.Score2, 
                    Score2 = string.Equals(game.Player1.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase) ? game.Score2 : game.Score1  })
                .Select(g =>
                {
                    if (g.Score1 == g.Score2)
                        return 'D';

                    return g.Score1 > g.Score2 ? 'W' : 'L';
                });
        }

        public decimal WinPercentage(string emailAddress)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress);

            if (gamesByPlayer.Count == 0)
                return 0;

            var won =  gamesByPlayer
                .Count(arg => arg.Score1 > arg.Score2);

            return (decimal) won / gamesByPlayer.Count;
        }

        public decimal SetWinPercentage(string emailAddress)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress);

            if (gamesByPlayer.Count == 0)
                return 0;

            var won =  gamesByPlayer
                .Sum(arg => arg.Score1);

            var totalSets = gamesByPlayer.Sum(arg => arg.Score1 + arg.Score2);

            return (decimal) won / totalSets;

        }

        public int RecordWinningStreak(string emailAddress)
        {
                var gamesByPlayer = GamesByPlayer(emailAddress);

                if (gamesByPlayer.Count == 0)
                    return 0;

                var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
                if (!series.Any())
                    return 0;

                return series.Select(games => games.Count()).Max();
        }

        public int CurrentWinningStreak(string emailAddress)
        {
            var gamesByPlayer = GamesByPlayer(emailAddress);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Last().Count();
        }

        public decimal RecordEloStreak(string emailAddress)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Select(games => games.Sum(game => game.Delta1)).Max() ?? 0;
        }

        public decimal CurrentEloStreak(string emailAddress)
        {
            var gamesByPlayer = EloGamesByPlayer(emailAddress);

            if (gamesByPlayer.Count == 0)
                return 0;

            var series = gamesByPlayer.Split(game => game.Score1 > game.Score2).ToList();
            if (!series.Any())
                return 0;

            return series.Last().Sum(games => games.Delta1.Value);
        }

        private List<StatGame> EloGamesByPlayer(string emailAddress)
        {
            return EloGames("tafeltennis", DateTime.MinValue, DateTime.MaxValue)
                .Where((game, i) => game.Game.Player1.EmailAddress.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase) || game.Game.Player2.EmailAddress.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase))
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

        private List<StatGame> GamesByPlayer(string emailAddress)
        {
            // TODO tafeltennis hardcoded...
            return _gamesService
                .List(new GamesForPlayerInPeriodSpecification("tafeltennis", emailAddress, DateTime.MinValue, DateTime.MaxValue))
                .Select(game => string.Equals(game.Player1.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase) 
                    ? new {game.Score1, game.Score2} : new {Score1 = game.Score2, Score2 = game.Score1})
                .Select(arg => new StatGame(arg.Score1, arg.Score2))
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
                    player => new EloStatsPlayer {EloScore = _eloConfiguration.InitialElo, NumberOfGames = 0, TimeNumberOne = new TimeSpan(0,0,0)});

            var lastGameRegistrationDate = eloGames.First().Game.RegistrationDate;
            Profile lastNumberOne = null;

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
                    var profile = ranking.First().Key;
                    if (profile == lastNumberOne)
                        eloStatsPlayers[profile].TimeNumberOne += diff;
                    lastNumberOne = profile;
                }
                lastGameRegistrationDate = eloGame.Game.RegistrationDate;
            }

            return eloStatsPlayers;
        }

        public IEnumerable<EloGame> EloGames(string gameType, DateTime startDate, DateTime endDate)
        {
            // All players should be in the ranking. Not restrictions (not yet :-))
            var allPlayers = _gamesService.List(new AllProfiles()).ToList();

            // All players have an initial elo score
            var ranking = allPlayers
                .ToDictionary(player => player,
                    player => new EloStatsPlayer {EloScore = _eloConfiguration.InitialElo, NumberOfGames = 0});

            // Now calculate current elo score based on all games played
            _logger.LogInformation("********* List all games");
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

                var player1Delta = CalculateDeltaFirstPlayer(oldRatingPlayer1.EloScore, oldRatingPlayer2.EloScore,
                    game.Score1,
                    game.Score2);

                var eloGames = new EloGame(game, ranking[game.Player1].EloScore, ranking[game.Player2].EloScore, player1Delta, -1* player1Delta);

                ranking[game.Player1].EloScore = oldRatingPlayer1.EloScore + player1Delta;
                ranking[game.Player2].EloScore = oldRatingPlayer2.EloScore - player1Delta;

                yield return eloGames;
            }
        }

        public KeyValuePair<DateTime, RankingStats> CalculateStats(DateTime startDate, DateTime endDate)
        {
            return _oldRankingOldStatisticsService.CalculateStats(startDate, endDate);
        }

        public ObsoleteRanking Ranking(string gameType, DateTime startDate, DateTime endDate)
        {
            return _oldRankingOldStatisticsService.Ranking(gameType, startDate, endDate);
        }

        public decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1,
            int gameScore2)
        {
            return _eloCalculator.CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);
        }
    }

    internal class StatGame
    {
        public int Score1 { get; }
        public int Score2 { get; }
        public decimal? Delta1 { get; }
        public decimal? Delta2 { get; }

        public StatGame(in int score1, in int score2, decimal? delta1 = null, decimal? delta2 = null)
        {
            Score1 = score1;
            Score2 = score2;
            Delta1 = delta1;
            Delta2 = delta2;
        }
    }

    public class EloGame
    {
        public Game Game { get; }
        public decimal EloPlayer1 { get; }
        public decimal EloPlayer2 { get; }
        public decimal Player1Delta { get; }
        public decimal Player2Delta { get; }

        public EloGame(Game game, decimal eloPlayer1, decimal eloPlayer2, decimal player1Delta, decimal player2Delta)
        {
            Game = game;
            EloPlayer1 = eloPlayer1;
            EloPlayer2 = eloPlayer2;
            Player1Delta = player1Delta;
            Player2Delta = player2Delta;
        }
    }

    public class EloStatsPlayer
    {
        public int Ranking { get; set; }
        public decimal EloScore { get; set; }
        public int NumberOfGames { get; set; }
        public TimeSpan TimeNumberOne { get; set; }
    }
}