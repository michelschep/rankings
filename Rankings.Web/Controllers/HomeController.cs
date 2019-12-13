using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly IStatisticsService _statisticsService;

        public HomeController(IStatisticsService rankingService)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            return View(new WhatIfModel());
        }

        public IActionResult WhatIf(WhatIfModel model)
        {
            var deltaFirstPlayer = _statisticsService.CalculateDeltaFirstPlayer(model.RatingPlayer1, model.RatingPlayer2, model.GameScore1, model.GameScore2);
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
            // TODO maybe better some build in types if it really means something different. Or constants to avoid strings all over the place.
            var ratings = _statisticsService.Ranking("tafeltennis").DeprecatedRatings;
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
            var deltaFirstPlayer = _statisticsService.CalculateDeltaFirstPlayer(thisPlayerElo, stats.Value.Ranking, gameScore1, gameScore2);
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
}
