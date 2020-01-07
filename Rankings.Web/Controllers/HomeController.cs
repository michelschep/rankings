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

        
        [HttpGet("/")]
        [HttpGet("/rankings")]
        public IActionResult Index()
        {
            return View(new WhatIfModel());
        }

        [HttpGet("/home/matrix")]
        [HttpGet("/home/matrix/{elo}")]
        public IActionResult Matrix(int? elo)
        {
            var model = new List<EloMatrixViewModel>();

            // TODO make it work for other types as well
            // TODO maybe better some build in types if it really means something different. Or constants to avoid strings all over the place.
            var ratings = _statisticsService
                .Ranking("tafeltennis", new DateTime(2020, 1, 1), DateTime.MaxValue);

            ratings.Add(new Profile() {DisplayName = "800"}, new EloStatsPlayer() {EloScore = 800});
            ratings.Add(new Profile() {DisplayName = "900"}, new EloStatsPlayer() {EloScore = 900});
            ratings.Add(new Profile() {DisplayName = "1000"}, new EloStatsPlayer() {EloScore = 1000});
            ratings.Add(new Profile() {DisplayName = "1100"}, new EloStatsPlayer() {EloScore = 1100});
            ratings.Add(new Profile() {DisplayName = "1200"}, new EloStatsPlayer() {EloScore = 1200});
            ratings.Add(new Profile() {DisplayName = "1300"}, new EloStatsPlayer() {EloScore = 1300});
            ratings.Add(new Profile() {DisplayName = "1400"}, new EloStatsPlayer() {EloScore = 1400});
            ratings.Add(new Profile() {DisplayName = "1500"}, new EloStatsPlayer() {EloScore = 1500});
            ratings.Add(new Profile() {DisplayName = "1600"}, new EloStatsPlayer() {EloScore = 1600});
            ratings.Add(new Profile() {DisplayName = "1700"}, new EloStatsPlayer() {EloScore = 1700});
            ratings.Add(new Profile() {DisplayName = "1800"}, new EloStatsPlayer() {EloScore = 1800});
            ratings.Add(new Profile() {DisplayName = "1900"}, new EloStatsPlayer() {EloScore = 1900});
            ratings.Add(new Profile() {DisplayName = "2000"}, new EloStatsPlayer() {EloScore = 2000});

            IEqualityComparer<KeyValuePair<Profile, EloStatsPlayer>> comparer = new MatrixComparer();
            ratings = new Dictionary<Profile, EloStatsPlayer>(ratings.Distinct(comparer));

            var thisPlayerElo = elo ?? ratings.First(pair => pair.Key.EmailAddress == User.Identity.Name).Value.EloScore;

            foreach (var stats in ratings.OrderByDescending(pair => pair.Value.EloScore))
            {
                var eloDiff = (int) Math.Round(stats.Value.EloScore - thisPlayerElo, 0, MidpointRounding.AwayFromZero);
                var draw = CalculateDeltaResult(thisPlayerElo, 1, 1, stats.Value.EloScore);
                var oneZeroWin = CalculateDeltaResult(thisPlayerElo, 1, 0, stats.Value.EloScore);
                var oneZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 1, stats.Value.EloScore);

                var twoZeroWin = CalculateDeltaResult(thisPlayerElo, 2, 0, stats.Value.EloScore);
                var twoZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 2, stats.Value.EloScore);

                var threeZeroWin = CalculateDeltaResult(thisPlayerElo, 3, 0, stats.Value.EloScore);
                var threeOneWin = CalculateDeltaResult(thisPlayerElo, 3, 1, stats.Value.EloScore);
                var threeTwoWin = CalculateDeltaResult(thisPlayerElo, 3, 2, stats.Value.EloScore);

                var threeZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 3, stats.Value.EloScore);
                var threeOneLost = CalculateDeltaResult(thisPlayerElo, 1, 3, stats.Value.EloScore);
                var threeTwoLost = CalculateDeltaResult(thisPlayerElo, 2, 3, stats.Value.EloScore);

                var line = new EloMatrixViewModel(stats.Key.DisplayName, (int) Math.Round(stats.Value.EloScore, 0, MidpointRounding.AwayFromZero), eloDiff, draw, 
                    oneZeroWin, oneZeroLost, twoZeroWin, twoZeroLost, threeZeroWin, 
                    threeZeroLost)
                {
                    ThreeOneWin = threeOneWin,
                    ThreeTwoWin = threeTwoWin,
                    ThreeOneLost = threeOneLost,
                    ThreeTwoLost = threeTwoLost,
                };
                model.Add(line);
            }

            return View(model);
        }

        private decimal CalculateDeltaResult(decimal thisPlayerElo, int gameScore1, int gameScore2, decimal eloScoreOponent)
        {
            var eloCalculator = new EloCalculatorVersion2020();
            var deltaFirstPlayer = eloCalculator.CalculateDeltaPlayer(thisPlayerElo, eloScoreOponent, gameScore1, gameScore2);
            return deltaFirstPlayer.Round(2);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }

    public class MatrixComparer : IEqualityComparer<KeyValuePair<Profile, EloStatsPlayer>>
    {
        public bool Equals(KeyValuePair<Profile, EloStatsPlayer> x, KeyValuePair<Profile, EloStatsPlayer> y)
        {
            return x.Value.EloScore == y.Value.EloScore;
        }

        public int GetHashCode(KeyValuePair<Profile, EloStatsPlayer> obj)
        {
            return obj.Value.EloScore.GetHashCode();
        }
    }
}