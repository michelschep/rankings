using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRankingService _rankingService;

        public HomeController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            var model = new WhatIfModel();
            return View(model);
        }

        public IActionResult WhatIf(WhatIfModel model)
        {
            var deltaFirstPlayer = _rankingService.CalculateDeltaFirstPlayer(model.RatingPlayer1, model.RatingPlayer2, model.GameScore1, model.GameScore2);
            model.Delta = Math.Round(deltaFirstPlayer, 2, MidpointRounding.AwayFromZero);
            model.ExpectedToWinSet = NewEloCalculator.ExpectationOneSet(model.RatingPlayer1, model.RatingPlayer2);
            model.ExpectedToWinGame = NewEloCalculator.CalculateExpectationForResult(model.RatingPlayer1, model.RatingPlayer2, model.GameScore1, model.GameScore2);

            ModelState.Clear();
            return View("Index", model);
        }
        
        public IActionResult Matrix()
        {
            var model = new List<EloMatrixViewModel>();

            // TODO make it work for other types as well
            // TODO maybe better some build in types if it really means something different. Or consts to avoid strings all over the place.
            var ratings = _rankingService.Ranking("tafeltennis").OldRatings;
            var thisPlayerElo = ratings.First(pair => pair.Key.EmailAddress == User.Identity.Name).Value.Ranking;
            foreach (var stats in ratings.OrderByDescending(pair => pair.Value.Ranking))
            {
                var eloDiff = (int) Math.Round(stats.Value.Ranking - thisPlayerElo, 0, MidpointRounding.AwayFromZero);
                var draw = CalculateDeltaResult(thisPlayerElo, stats, 1, 1);
                var oneZeroWin = CalculateDeltaResult(thisPlayerElo, stats, 1, 0);
                var oneZeroLost = CalculateDeltaResult(thisPlayerElo, stats, 0, 1);

                var twoZeroWin = CalculateDeltaResult(thisPlayerElo, stats, 2, 0);
                var twoZeroLost = CalculateDeltaResult(thisPlayerElo, stats, 0, 2);
                
                var threeZeroWin = CalculateDeltaResult(thisPlayerElo, stats, 3, 0);
                var threeZeroLost = CalculateDeltaResult(thisPlayerElo, stats, 0, 3);

                var line = new EloMatrixViewModel(stats.Key.DisplayName, (int) Math.Round(stats.Value.Ranking, 0, MidpointRounding.AwayFromZero), eloDiff, draw, oneZeroWin, oneZeroLost, twoZeroWin, twoZeroLost, threeZeroWin, threeZeroLost);                
                model.Add(line);
            }
            
            return View(model);
        }

        private int CalculateDeltaResult(decimal thisPlayerElo, KeyValuePair<Profile, PlayerStats> stats, int gameScore1, int gameScore2)
        {
            var deltaFirstPlayer = _rankingService.CalculateDeltaFirstPlayer(thisPlayerElo, stats.Value.Ranking, gameScore1, gameScore2);
            return (int) Math.Round(deltaFirstPlayer, 0, MidpointRounding.AwayFromZero);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class EloMatrixViewModel
    {
        public string Name { get; }
        public int Elo { get; }
        public int EloDiff { get; }
        public int Draw { get; }

        [DisplayName("1-0")]
        public int OneZeroWin { get; }
        
        [DisplayName("0-1")]
        public int OneZeroLost { get; }
        
        [DisplayName("2-0")]
        public int TwoZeroWin { get; }
        
        [DisplayName("0-2")]
        public int TwoZeroLost { get; }
        
        [DisplayName("3-0")]
        public int ThreeZeroWin { get; }
        
        [DisplayName("0-3")]
        public int ThreeZeroLost { get; }

        public EloMatrixViewModel(string name, int elo, int eloDiff, int draw, int oneZeroWin, int oneZeroLost, int twoZeroWin, int twoZeroLost, int threeZeroWin, int threeZeroLost)
        {
            Name = name;
            Elo = elo;
            EloDiff = eloDiff;
            Draw = draw;
            OneZeroWin = oneZeroWin;
            OneZeroLost = oneZeroLost;
            TwoZeroWin = twoZeroWin;
            TwoZeroLost = twoZeroLost;
            ThreeZeroWin = threeZeroWin;
            ThreeZeroLost = threeZeroLost;
        }
    }
}
