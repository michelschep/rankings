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

            Dictionary<Profile, decimal> ratings = new Dictionary<Profile, decimal>();
            foreach (var profile in games.SelectMany(game => new List<Profile> { game.Player1, game.Player2 }).Distinct())
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
                var factor = game.Score1 == game.Score2 ? game.Score1 + game.Score2 : 2 * max - 1;

                var fibo = new int[6] {0, 1, 1, 2, 3, 5};
                factor = factor > 5 ? 5 : fibo[factor];

                //K = 42;//27 + 5 * factor;
                K = 27 + 5 * factor;
                K = 42+5 +3;
                K = 42 + 5*(game.Score1 + game.Score2);
                //K = 100;
                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];
                decimal expectedOutcome1 = CalculateExpectation(oldRatingPlayer1, oldRatingPlayer2);
                decimal expectedOutcome2 = 1 - expectedOutcome1;//CalculateExpectation(oldRatingPlayer2, oldRatingPlayer1);

                var test = expectedOutcome2 + expectedOutcome1;
                //decimal actual1 = game.Score1 / ((decimal)game.Score1 + (decimal)game.Score2);
                //decimal actual1 = game.Score1 > game.Score2 ? 1 : 0;
                decimal actual1 = game.Score1 > game.Score2 ? 1 : 0;
                //actual1 += game.Score1 / ((decimal) game.Score1 + (decimal) game.Score2);
                //actual1 = actual1 / 2;

                decimal winnerEloDiff = game.Score1 > game.Score2
                    ? oldRatingPlayer1 - oldRatingPlayer2
                    : oldRatingPlayer2 - oldRatingPlayer1;

                var marginOfVicoryMultiplier = (decimal)Math.Log(Math.Abs(game.Score1 - game.Score2) + 1) * (2.2m / (winnerEloDiff * 0.001m + 2.2m));

                decimal actual2 = 1 - actual1;//Math.Round(game.Score2 / ((decimal)game.Score1 + (decimal)game.Score2), 2);
                var outcome1 = (actual1 - expectedOutcome1);
                var player1Delta = K * outcome1 * marginOfVicoryMultiplier;
                decimal newRatingPlayer1 = oldRatingPlayer1 + player1Delta;

                //var outcome2 = (actual2 - expectedOutcome2);
                //var player2Delta = K * outcome2 * marginOfVicoryMultiplier;
                decimal newRatingPlayer2 = oldRatingPlayer2 - player1Delta;

                var r = newRatingPlayer1 + newRatingPlayer2;

                ratings[game.Player1] = newRatingPlayer1;
                ratings[game.Player2] = newRatingPlayer2;
            }

            var model = ratings
                .OrderByDescending(pair => pair.Value)
                .Select(r => new RankingViewModel {Points = (int)Math.Round(r.Value,0,MidpointRounding.AwayFromZero), NamePlayer = r.Key.DisplayName, Ranking = ranking++});

            return View(model);
        }

        private decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            decimal n = 400;
            decimal x = oldRatingPlayer1 - oldRatingPlayer2;
            decimal exponent = -1 * (x / n);
            decimal expected = (decimal)(1 / (1 + Math.Pow(10, (double)exponent)));

            return expected;
        }
    }
}