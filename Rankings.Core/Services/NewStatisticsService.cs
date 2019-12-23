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
    public class NewStatisticsService : IStatisticsService
    {
        private readonly IGamesService _gamesService;//todo
        private readonly EloConfiguration _eloConfiguration;//todo
        private readonly ILogger<IStatisticsService> _logger;//todo
        private readonly EloCalculator _eloCalculator;//todo
        private readonly OldStatisticsService _oldRankingOldStatisticsService;//todo

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

        public Dictionary<Profile, decimal> EloStats(string gameType, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Calculate ELO new");

            // All players should be in the ranking. Not restrictions (not yet :-))
            var allPlayers = _gamesService.List(new AllProfiles());

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
                //_logger.LogTrace($"Game {game.Player1.DisplayName}-{game.Player2.DisplayName}: {game.Score1}-{game.Score2}");

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

                //_logger.LogTrace($"p1 {oldRatingPlayer1}+{player1Delta} = {ranking[game.Player1]}");
                //_logger.LogTrace($"p2 {oldRatingPlayer2}+{-1*player1Delta} = {ranking[game.Player2]}");
            }

            return ranking;
        }

        public KeyValuePair<DateTime, RankingStats> CalculateStats(DateTime startDate, DateTime endDate)
        {
            return _oldRankingOldStatisticsService.CalculateStats(startDate, endDate);
        }

        public ObsoleteRanking Ranking(string gameType, DateTime startDate, DateTime endDate)
        {
            return _oldRankingOldStatisticsService.Ranking(gameType, startDate, endDate);
        }

        public decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            return _eloCalculator.CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);
        }
    }
}