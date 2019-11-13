using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class RankingsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public RankingsController(IStatisticsService rankingService)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        [HttpGet("/rankings")]
        [HttpGet("/rankings/{gametype}/{endDateInput}")]
        public IActionResult Index(string gameType, string endDateInput)
        {
            gameType = gameType ?? "tafeltennis";
            var endDate = endDateInput == null ? DateTime.MaxValue : DateTime.Parse(endDateInput);
            
            var ratings = _statisticsService.Ranking(gameType, DateTime.MinValue, endDate);
            var ranking = 1;
            var numberOfGames = gameType == "tafeltennis" ? 6 : 0;
            //numberOfGames = User.HasClaim(ClaimTypes.Role, "Admin") ? 0 : numberOfGames;

            var model = ratings.DeprecatedRatings.Where(pair => pair.Value.NumberOfGames >= numberOfGames)
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    NumberOfGames = r.Value.NumberOfGames,
                    WinPercentage = Math.Round(r.Value.WinPercentage,0,MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture),
                    SetWinPercentage = Math.Round(r.Value.SetWinPercentage, 0, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture),
                    Points = Math.Round(r.Value.Ranking,0,MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture), 
                    NamePlayer = r.Key.DisplayName, 
                    Ranking = (ranking++) + ".",
                    History = ToHistory(r),
                    RecordWinningStreak = ToWinningStreak(r),
                    RecordEloStreak = (int) r.Value.BestEloSeries
                });

            Response.Headers.Add("Refresh", "30");
            return View(model);
        }

        private int ToWinningStreak(in KeyValuePair<Profile, PlayerStats> r)
        {
            return r.Value.History.Split('L').Select(s => s.Length).Max();
        }

        private static List<char> ToHistory(KeyValuePair<Profile, PlayerStats> r)
        {
            var chars = r.Value.History.ToCharArray().Reverse().ToList();
            return chars.Take(7).Reverse().ToList();
        }
    }
}