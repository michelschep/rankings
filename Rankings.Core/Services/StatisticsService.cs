using Microsoft.Extensions.Logging;
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

        public StatisticsService(IGamesService gamesService, EloConfiguration eloConfiguration, ILogger<IStatisticsService> logger)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _eloConfiguration = eloConfiguration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDictionary<Profile, EloStatsPlayer> Ranking(string gameType, DateTime startDate, DateTime endDate)
        {
            var eloStatsPlayers = EloStatsPlayers(gameType, startDate, endDate);
            var orderedRanking = CalculateRanking(eloStatsPlayers, _eloConfiguration.NumberOfGames);

            return orderedRanking;
        }

        private Dictionary<Profile, EloStatsPlayer> CalculateRanking(
            Dictionary<Profile, EloStatsPlayer> eloStatsPlayers, int numberOfGames)
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
                        ? new {game.Score1, game.Score2}
                        : new {Score1 = game.Score2, Score2 = game.Score1})
                .Select(arg => new StatGame(arg.Score1, arg.Score2))
                .ToList();
        }

        public IDictionary<Profile, decimal> GoatScores(string tafeltennis, in DateTime minValue, in DateTime maxValue)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Profile, EloStatsPlayer> EloStatsPlayers(string gameType, DateTime startDate,
            DateTime endDate)
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
                    var currentNumberOne = ranking.First().Key;

                    if (lastNumberOne != null) {
                        eloStatsPlayers[lastNumberOne].TimeNumberOne += diff;
//                        _logger.LogInformation($"{lastNumberOne.EmailAddress} {eloStatsPlayers[lastNumberOne].TimeNumberOne}");
                    }

                   // if (lastNumberOne != currentNumberOne)
                   // {
                   //     _logger.LogInformation( $" New nr1: {currentNumberOne.EmailAddress} {eloGame.Game.RegistrationDate} ==> {eloStatsPlayers[currentNumberOne].TimeNumberOne}");
                   //     if (lastNumberOne != null)
                   //      _logger.LogInformation( $" Old nr1: {lastNumberOne.EmailAddress} ==> {eloStatsPlayers[lastNumberOne].TimeNumberOne}");
                   // }

                    lastNumberOne = currentNumberOne;
                }

                lastGameRegistrationDate = eloGame.Game.RegistrationDate;
            }

            var lastDate = DateTime.Now < endDate ? DateTime.Now : endDate;
            var diff2 = eloGames.Last().Game.RegistrationDate - lastGameRegistrationDate;
            eloStatsPlayers[lastNumberOne].TimeNumberOne += diff2;

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

            var calculator2019 = new EloCalculatorVersion2019();
            var calculator2020 = new EloCalculatorVersion2020();
            IEloCalculator eloCalculator = calculator2019;

            // Now calculate current elo score based on all games played
            _logger.LogInformation("********* List all games");
            var games = _gamesService.List(new GamesForPeriodSpecification(gameType, startDate, endDate)).ToList();
            foreach (var game in games.OrderBy(game => game.RegistrationDate))
            {
                //eloCalculator = game.RegistrationDate.Year == 2019 ? (IEloCalculator) calculator2019 : calculator2020;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ranking[game.Player1];
                var oldRatingPlayer2 = ranking[game.Player2];

                var player1Delta = eloCalculator.CalculateDeltaPlayer(oldRatingPlayer1.EloScore,
                    oldRatingPlayer2.EloScore, game.Score1, game.Score2);
                var player2Delta = eloCalculator.CalculateDeltaPlayer(oldRatingPlayer2.EloScore,
                    oldRatingPlayer1.EloScore, game.Score2, game.Score1);

                var eloGames = new EloGame(game, ranking[game.Player1].EloScore, ranking[game.Player2].EloScore,
                    player1Delta, player2Delta);

                ranking[game.Player1].EloScore = oldRatingPlayer1.EloScore + player1Delta;
                ranking[game.Player2].EloScore = oldRatingPlayer2.EloScore + player2Delta;

                yield return eloGames;
            }
        }
    }
}