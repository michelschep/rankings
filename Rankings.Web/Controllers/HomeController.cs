using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            model.ExpectedToWinGame = NewEloCalculator.CalculateExpectation(model.RatingPlayer1, model.RatingPlayer2, model.GameScore1, model.GameScore2);

            ModelState.Clear();
            return View("Index", model);
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
