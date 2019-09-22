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
            var model = _rankingService
                .Profiles()
                .Select(profile => new RankingViewModel {Points = 1200, NamePlayer = profile.DisplayName, Ranking = ranking++});

            return View(model);
        }
    }
}