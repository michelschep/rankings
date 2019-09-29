using System;
using System.Collections.Generic;
using System.Linq;
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
            var ratings = _rankingService.Ranking();
            var ranking = 1;

            var model = ratings
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    WinPercentage = (int)Math.Round((100m*r.Value.NumberOfWins/r.Value.NumberOfGames), 0, MidpointRounding.AwayFromZero),
                    SetWinPercentage = (int)Math.Round((100m*r.Value.NumberOfSetWins/r.Value.NumberOfSets), 0, MidpointRounding.AwayFromZero),
                    Points = (int)Math.Round(r.Value.Ranking,0,MidpointRounding.AwayFromZero), 
                    NamePlayer = r.Key.DisplayName, 
                    Ranking = ranking++,
                    History = r.Value.History.ToCharArray().Reverse().ToList().Take(3).Reverse().ToList()//r.Value.History.Substring(r.Value.History.Length - 3 < 0 ? 0 : r.Value.History.Length-3)//Reverse().Take(3).Reverse().
                });

            return View(model);
        }

    }

}