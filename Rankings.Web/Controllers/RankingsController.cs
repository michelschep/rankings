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

            Dictionary<Profile, PlayerStats> ratings = new Dictionary<Profile, PlayerStats>();
            foreach (var profile in games.SelectMany(game => new List<Profile> { game.Player1, game.Player2 }).Distinct())
            {
                ratings.Add(profile, new PlayerStats()
                {
                    NumberOfGames = 0,
                    NumberOfSetWins = 0,
                    NumberOfSets = 0,
                    NumberOfWins = 0,
                    Ranking = 1200,
                    //History = ""
                });
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
                K = 50;
                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];
                decimal expectedOutcome1 = CalculateExpectation(oldRatingPlayer1.Ranking, oldRatingPlayer2.Ranking);
                decimal expectedOutcome2 = 1 - expectedOutcome1;//CalculateExpectation(oldRatingPlayer2, oldRatingPlayer1);

                var test = expectedOutcome2 + expectedOutcome1;
                //decimal actual1 = game.Score1 / ((decimal)game.Score1 + (decimal)game.Score2);
                //decimal actual1 = game.Score1 > game.Score2 ? 1 : 0;
                decimal actual1 = game.Score1 > game.Score2 ? 1 : 0;
                //actual1 += game.Score1 / ((decimal) game.Score1 + (decimal) game.Score2);
                //actual1 = actual1 / 2;

                decimal winnerEloDiff = game.Score1 > game.Score2
                    ? oldRatingPlayer1.Ranking - oldRatingPlayer2.Ranking
                    : oldRatingPlayer2.Ranking - oldRatingPlayer1.Ranking;

                var marginOfVicoryMultiplier = (decimal)Math.Log(Math.Abs(game.Score1 - game.Score2) + 1) * (2.2m / (winnerEloDiff * 0.001m + 2.2m));

                decimal actual2 = 1 - actual1;//Math.Round(game.Score2 / ((decimal)game.Score1 + (decimal)game.Score2), 2);
                var outcome1 = (actual1 - expectedOutcome1);
                var player1Delta = K * outcome1 * marginOfVicoryMultiplier;
                decimal newRatingPlayer1 = oldRatingPlayer1.Ranking + player1Delta;

                //var outcome2 = (actual2 - expectedOutcome2);
                //var player2Delta = K * outcome2 * marginOfVicoryMultiplier;
                decimal newRatingPlayer2 = oldRatingPlayer2.Ranking - player1Delta;

                var r = newRatingPlayer1 + newRatingPlayer2;

                ratings[game.Player1].Ranking = newRatingPlayer1;
                ratings[game.Player2].Ranking = newRatingPlayer2;

                ratings[game.Player1].NumberOfGames += 1;
                ratings[game.Player2].NumberOfGames += 1;

                ratings[game.Player1].NumberOfWins += game.Score1 > game.Score2 ? 1 : 0;
                ratings[game.Player2].NumberOfWins += game.Score2 > game.Score1 ? 1 : 0;

//                if (game.Score1 > game.Score2)
//                {
//                    ratings[game.Player1].History += "W";
//                    ratings[game.Player2].History += "L";
//                }
//
//                if (game.Score1 == game.Score2)
//                {
//                    ratings[game.Player1].History += "D";
//                    ratings[game.Player2].History += "D";
//                }

                ratings[game.Player1].NumberOfSets += game.Score1 + game.Score2;
                ratings[game.Player2].NumberOfSets += game.Score1 + game.Score2;

                ratings[game.Player1].NumberOfSetWins += game.Score1;
                ratings[game.Player2].NumberOfSetWins += game.Score2;
            }

            var model = ratings
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    WinPercentage = (int)Math.Round((100m*r.Value.NumberOfWins/r.Value.NumberOfGames), 0, MidpointRounding.AwayFromZero),
                    SetWinPercentage = (int)Math.Round((100m*r.Value.NumberOfSetWins/r.Value.NumberOfSets), 0, MidpointRounding.AwayFromZero),
                    Points = (int)Math.Round(r.Value.Ranking,0,MidpointRounding.AwayFromZero), 
                    NamePlayer = r.Key.DisplayName, 
                    Ranking = ranking++,
                    //History = r.Value.History.Substring(r.Value.History.Length - 3)//Reverse().Take(3).Reverse().
                });

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

    public class PlayerStats
    {
        public decimal Ranking { get; set; }
        public int NumberOfGames { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfSets { get; set; }
        public int NumberOfSetWins { get; set; }
        //public string History { get; set; }
    }
}