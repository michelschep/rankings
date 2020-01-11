using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class RankingsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMemoryCache _memoryCache;

        public RankingsController(IStatisticsService rankingService, IMemoryCache memoryCache)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet("/rankings/{year}")]
        public IActionResult YearRanking(int year)
        {
            ViewBag.Title = $"The {year} Ranking";
            ViewBag.Message = "You need at least 7 games to be in the ranking";

            if (year == 2020)
                ViewBag.Message += " (but for January everybody who played at least one game will be shown)";

            var gameType = "tafeltennis";
            var beginEnd = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);
            
            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType + "-" + year, entry =>
            {
                var model = RankingViewModelsFor(gameType, beginEnd, endDate)
                    .ToList();
                return model;
            });

            return View("Index", cacheEntry);
        }

        private bool IsAdmin()
        {
            return User.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == Roles.Admin);
        }

        [HttpGet("/rankings/eternal")]
        public IActionResult EternalRanking()
        {
            var gameType = "tafeltennis";
            var beginEnd = DateTime.MinValue; 
            var endDate = DateTime.MaxValue; 
            
            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType + "-eternal", entry =>
            {
                var model = RankingViewModelsFor(gameType, beginEnd, endDate, 0)
                    .ToList();
                return model;
            });

            this.ViewBag.Title = "The Eternal Ranking";
            this.ViewBag.Message = "You need at least 7 games to be in the ranking";

            return View("Index", cacheEntry);
        }

        private IEnumerable<RankingViewModel> RankingViewModelsFor(string gameType, DateTime startDate, DateTime endDate, int precision = 0)
        {
            return NewRankingViewModels(gameType, startDate, endDate, precision);
        }

        private IEnumerable<RankingViewModel> NewRankingViewModels(string gameType, DateTime startDate, DateTime endDate, int precision)
        {
            // Determine list of players with elo score
            var eloScores = _statisticsService.Ranking(gameType, startDate, endDate);

            // Fill view model with elo score
            var ranking = 1;
            int minimalNumberOfGames = startDate.Year <= 2019 ? 7 : 1;
            var list = eloScores
                .Where((pair, i) => pair.Value.NumberOfGames >= minimalNumberOfGames)
                // TODO use id (guid) in stead of email address
                .Select(pair => new RankingViewModel
                {
                    EmailAddress = pair.Key.EmailAddress, 
                    NamePlayer = pair.Key.DisplayName, 
                    Points = pair.Value.EloScore.Round(precision), 
                    Ranking = CreateIcon(ranking++),
                    TimeNumberOne = pair.Value.TimeNumberOne > new TimeSpan(0) ? pair.Value.TimeNumberOne.ToString(@"d\.hh\:mm") : ""
                })
                .ToList();

            foreach (var rankingViewModel in list)
            {
                rankingViewModel.History = _statisticsService.History(rankingViewModel.EmailAddress, startDate, endDate).ToList();
                rankingViewModel.WinPercentage = (100* _statisticsService.WinPercentage(rankingViewModel.EmailAddress, startDate, endDate)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.SetWinPercentage = (100* _statisticsService.SetWinPercentage(rankingViewModel.EmailAddress, startDate, endDate)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.RecordWinningStreak = _statisticsService.RecordWinningStreak(rankingViewModel.EmailAddress, startDate, endDate);
                rankingViewModel.CurrentWinningStreak = _statisticsService.CurrentWinningStreak(rankingViewModel.EmailAddress, startDate, endDate);
                rankingViewModel.RecordEloStreak = (int) _statisticsService.RecordEloStreak(rankingViewModel.EmailAddress, startDate, endDate).Round();
                rankingViewModel.CurrentEloStreak = (int) _statisticsService.CurrentEloStreak(rankingViewModel.EmailAddress, startDate, endDate).Round();
            }

            return list;
        }

        private string CreateIcon(int ranking)
        {
            if (ranking == 1)
                return "🥇";

            if (ranking == 2)
                return "🥈";

            if (ranking == 3)
                return "🥉";

            return ranking.ToString();
        }
    }
}