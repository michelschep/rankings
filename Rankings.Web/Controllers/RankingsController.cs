using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class RankingsController : Controller
    {
        private readonly IRankingService _rankingService;

        public RankingsController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            var ranking = 1;
            var K = 32;
            var players = _rankingService.Profiles().ToList();

            // TODO fix loading entities
            var gameTypes = _rankingService.GameTypes();
            var venues = _rankingService.GetVenues();

            var enumerable = _rankingService.Games().ToList();
            var games = enumerable
                .Where(game => game.GameType.Code == "tafeltennis")
                .OrderBy(game => game.RegistrationDate)
                .ToList();

            Dictionary<Profile, int> ratings = new Dictionary<Profile, int>();
            foreach (var profile in players)
            {
                ratings.Add(profile, 1200);
            }

            foreach (var game in games)
            {
                // TODO for tafeltennis a 0-0 is not a valid result. For time related games it is possible
                // For now ignore a 0-0
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var max = Math.Max(game.Score1, game.Score2);
                var factor = game.Score1 == game.Score2 ? game.Score1 + game.Score2 : 2 * max + 1;
                K = 27 + 5*factor;
                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];
                decimal expectedOutcome1 = CalculateExpectation(oldRatingPlayer1, oldRatingPlayer2);
                decimal expectedOutcome2 = 1 -expectedOutcome1;//CalculateExpectation(oldRatingPlayer2, oldRatingPlayer1);

                var test = expectedOutcome2 + expectedOutcome1;
                decimal actual1 = game.Score1 / ((decimal)game.Score1 + (decimal)game.Score2);
                decimal actual2 = 1 - actual1;//Math.Round(game.Score2 / ((decimal)game.Score1 + (decimal)game.Score2), 2);
                decimal newRatingPlayer1 = oldRatingPlayer1 + K * (actual1 - expectedOutcome1);
                decimal newRatingPlayer2 = oldRatingPlayer2 + K * (actual2 - expectedOutcome2);

                var r = K * (actual1 - expectedOutcome1) + K * (actual2 - expectedOutcome2);

                ratings[game.Player1] = (int)Math.Round(newRatingPlayer1, 0, MidpointRounding.AwayFromZero);
                ratings[game.Player2] = (int)Math.Round(newRatingPlayer2, 0, MidpointRounding.AwayFromZero); ;
            }

            var model = ratings
                .OrderByDescending(pair => pair.Value)
                .Select(r => new RankingViewModel {Points = r.Value, NamePlayer = r.Key.DisplayName, Ranking = ranking++});

            return View(model);
        }

        private decimal CalculateExpectation(int oldRatingPlayer1, int oldRatingPlayer2)
        {
            var n = 400;
            int x = oldRatingPlayer1 - oldRatingPlayer2;
            double exponent = -1 * (x / n);
            decimal expected = 1 / (1 + (decimal)Math.Pow(10, exponent));

            return Math.Round(expected, 2,MidpointRounding.AwayFromZero);
        }
    }
}