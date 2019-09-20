using System;
using System.Collections.Generic;
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
            List<RankingViewModel> model = new List<RankingViewModel>
            {
                new RankingViewModel {Points = 1200, NamePlayer = "Michel", Ranking = 1},
                new RankingViewModel {Points = 1200, NamePlayer = "Geale", Ranking = 2},
                new RankingViewModel {Points = 1200, NamePlayer = "Johannes", Ranking = 3},
                new RankingViewModel {Points = 1200, NamePlayer = "Arjen", Ranking = 4},
                new RankingViewModel {Points = 1200, NamePlayer = "Hans", Ranking = 5}
            };

            return View(model);
        }
    }
}