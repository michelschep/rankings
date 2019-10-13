using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Interfaces;
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
            var expected = 0m;//_rankingService.CalculateExpectation(model.RatingPlayer1, model.RatingPlayer2);//, model.GameScore1 + model.GameScore2);

            model.Delta = Math.Round(deltaFirstPlayer, 2, MidpointRounding.AwayFromZero);
            // TODO is expected to win game
            model.ExpectedToWinSet = Math.Round(expected, 2, MidpointRounding.AwayFromZero);
            model.ExpectedToWinGame = ChangeWinningGameWithSpecifiedResult(model.GameScore1, model.GameScore2, model.ExpectedToWinSet);

            var x = ChangeWinningGame(5, model.ExpectedToWinSet);

            ModelState.Clear();
            return View("Index", model);
        }

        private double ChangeWinningGame(int bestOf, decimal expectedToWinSet)
        {
            var a = ChangeWinningGameWithSpecifiedResult(3, 0, expectedToWinSet);
            var b = ChangeWinningGameWithSpecifiedResult(2, 1, expectedToWinSet);
            var c = ChangeWinningGameWithSpecifiedResult(1, 2, expectedToWinSet);
            var d = ChangeWinningGameWithSpecifiedResult(0, 3, expectedToWinSet);

            return a + b;
        }

        private double ChangeWinningGameWithSpecifiedResult(int gameScore1, int gameScore2, decimal expectedToWinSet)
        {
            var numberOfSets = gameScore1 + gameScore2;

            return (double)Factorial(numberOfSets) / (double)(Factorial(gameScore1) * Factorial(gameScore2)) 
                   * Math.Pow((double)expectedToWinSet, gameScore1) 
                   * Math.Pow((double)(1-expectedToWinSet), gameScore2);
        }

        private int Factorial(int numberOfSets)
        {
            if (numberOfSets == 0)
                return 1;

            if (numberOfSets == 1)
                return 1;

            return numberOfSets * Factorial(numberOfSets - 1);
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
