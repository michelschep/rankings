using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
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

            var model = ratings.OldRatings.Where(pair => pair.Value.NumberOfGames >= 5)
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    WinPercentage = Math.Round((100m*r.Value.NumberOfWins/r.Value.NumberOfGames), 0, MidpointRounding.AwayFromZero).ToString(),
                    SetWinPercentage = Math.Round((100m*r.Value.NumberOfSetWins/r.Value.NumberOfSets), 0, MidpointRounding.AwayFromZero).ToString(),
                    Points = Math.Round(r.Value.Ranking,0,MidpointRounding.AwayFromZero).ToString(), 
                    NamePlayer = r.Key.DisplayName, 
                    Ranking = (ranking++) + ".",
                    History = ToHistory(r)
                });

            var unranked = ratings.OldRatings.Where(pair => pair.Value.NumberOfGames < 5)
                .OrderByDescending(pair => pair.Value.NumberOfGames)
                .Select(r => new RankingViewModel
                {
                    WinPercentage = "-",
                    SetWinPercentage = "-",
                    Points = "-",
                    NamePlayer = r.Key.DisplayName + " (" + r.Value.NumberOfGames + ")",
                    Ranking = "",
                    History = new List<char>()
                });

            return View(model.Union(unranked));
        }

        private static List<char> ToHistory(KeyValuePair<Profile, PlayerStats> r)
        {
            return r.Value.History.ToCharArray().Reverse().ToList().Take(5).Reverse().ToList();
        }
    }
}