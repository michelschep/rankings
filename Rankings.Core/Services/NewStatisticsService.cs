﻿using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rankings.Core.Services
{
    public class NewStatisticsService : IStatisticsService
    {
        private readonly IGamesService _gamesService; //todo
        private readonly EloConfiguration _eloConfiguration; //todo
        private readonly ILogger<IStatisticsService> _logger; //todo
        private readonly EloCalculator _eloCalculator; //todo
        private readonly OldStatisticsService _oldRankingOldStatisticsService; //todo

        public NewStatisticsService(IGamesService gamesService, EloConfiguration eloConfiguration,
            ILogger<IStatisticsService> logger, EloCalculator eloCalculator,
            OldStatisticsService oldRankingOldStatisticsService)
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

            var rankedPlayers = eloStatsPlayers
                .Where(pair => pair.Value.NumberOfGames >= 7)
                .OrderByDescending(pair => pair.Value.EloScore);
            var orderedRanking = new Dictionary<Profile, EloStatsPlayer>(rankedPlayers);

            var index = 1;
            foreach (var item in orderedRanking.Values)
            {
                item.Ranking = index++;
            }

            return orderedRanking;
        }

        public IDictionary<Profile, decimal> GoatScores(string tafeltennis, in DateTime minValue, in DateTime maxValue)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Profile, EloStatsPlayer> EloStatsPlayers(string gameType, DateTime startDate, DateTime endDate)
        {
            var eloGames = EloGames(gameType, startDate, endDate);
            var allPlayers = _gamesService.List(new AllProfiles()).ToList();

            // All players have an initial elo score
            var eloStatsPlayers = allPlayers
                .ToDictionary(player => player,
                    player => new EloStatsPlayer {EloScore = _eloConfiguration.InitialElo, NumberOfGames = 0});

            foreach (var eloGame in eloGames)
            {
                var player1 = eloStatsPlayers[eloGame.Game.Player1];
                var player2 = eloStatsPlayers[eloGame.Game.Player2];

                player1.EloScore += eloGame.Player1Delta;
                player1.NumberOfGames += 1;
                player2.EloScore += eloGame.Player2Delta;
                player2.NumberOfGames += 1;
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
        public decimal EloScore { get; set; }
        public int Ranking { get; set; }
        public int NumberOfGames { get; set; }
    }
}