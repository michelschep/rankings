using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Matrix()
        {
            var model = new List<EloMatrixViewModel>();

            // TODO make it work for other types as well
            // TODO maybe better some build in types if it really means something different. Or constants to avoid strings all over the place.
            var ratings = _statisticsService
                .Ranking("tafeltennis", DateTime.MinValue, DateTime.MaxValue)
                .OrderBy(pair => pair.Value.EloScore);
            var thisPlayerElo = 1200;//ratings.First(pair => pair.Key.EmailAddress == User.Identity.Name).Value.Ranking;
            foreach (var stats in ratings.OrderByDescending(pair => pair.Value.Ranking))
            {
                var eloDiff = (int) Math.Round(stats.Value.EloScore - thisPlayerElo, 0, MidpointRounding.AwayFromZero);
                var draw = CalculateDeltaResult(thisPlayerElo, 1, 1, stats.Value.EloScore);
                var oneZeroWin = CalculateDeltaResult(thisPlayerElo, 1, 0, stats.Value.EloScore);
                var oneZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 1, stats.Value.EloScore);

                var twoZeroWin = CalculateDeltaResult(thisPlayerElo, 2, 0, stats.Value.EloScore);
                var twoZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 2, stats.Value.EloScore);
                
                var threeZeroWin = CalculateDeltaResult(thisPlayerElo, 3, 0, stats.Value.EloScore);
                var threeZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 3, stats.Value.EloScore);

                var line = new EloMatrixViewModel(stats.Key.DisplayName, (int) Math.Round(stats.Value.EloScore, 0, MidpointRounding.AwayFromZero), eloDiff, draw, oneZeroWin, oneZeroLost, twoZeroWin, twoZeroLost, threeZeroWin, threeZeroLost);                
                model.Add(line);
            }
            
            return View(model);
        }

        private int CalculateDeltaResult(decimal thisPlayerElo, int gameScore1, int gameScore2, decimal eloScoreOponent)
        {
            var eloCalculator = new EloCalculatorVersion2020();
            var deltaFirstPlayer = eloCalculator.CalculateDeltaPlayer(thisPlayerElo, eloScoreOponent, gameScore1, gameScore2);
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
