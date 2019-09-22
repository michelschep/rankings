using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
            var enumerable = _rankingService.Games().ToList();
            var games = enumerable
                .Where(game => game.GameType.Code == "tafeltennis")
                .OrderBy(game => game.RegistrationDate).ToList();

            Dictionary<string, int> ratings = new Dictionary<string, int>();
            foreach (var profile in players)
            {
                ratings.Add(profile.EmailAddress, 1200);
            }

            foreach (var game in games)
            {
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                K = 32 + 3*(game.Score1 + game.Score2);
                var oldRatingPlayer1 = ratings[game.Player1.EmailAddress];
                var oldRatingPlayer2 = ratings[game.Player2.EmailAddress];
                decimal expectedOutcome1 = CalculateExpectation(oldRatingPlayer1, oldRatingPlayer2);
                decimal expectedOutcome2 = CalculateExpectation(oldRatingPlayer2, oldRatingPlayer1);
                decimal actual1 = Math.Round(game.Score1 / ((decimal)game.Score1 + (decimal)game.Score2), 2);
                decimal actual2 = Math.Round(game.Score2 / ((decimal)game.Score1 + (decimal)game.Score2), 2);
                decimal newRatingPlayer1 = oldRatingPlayer1 + K * (actual1 - expectedOutcome1);
                decimal newRatingPlayer2 = oldRatingPlayer2 + K * (actual2 - expectedOutcome2);

                ratings[game.Player1.EmailAddress] = (int) newRatingPlayer1;
                ratings[game.Player2.EmailAddress] = (int) newRatingPlayer2;
            }

            var model = ratings
                .OrderByDescending(pair => pair.Value)
                .Select(r => new RankingViewModel {Points = r.Value, NamePlayer = r.Key, Ranking = ranking++});

            return View(model);
        }

        private decimal CalculateExpectation(int oldRatingPlayer1, int oldRatingPlayer2)
        {
            var n = 400;
            double x = oldRatingPlayer1 - oldRatingPlayer2;
            double exponent = -1 * (x / n);
            decimal expected = 1 / (1 + (decimal)Math.Pow(10, exponent));

            return expected;
        }
    }
}