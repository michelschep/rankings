using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        [HttpGet("/rankings/{gametype}/{endDateInput}")]
        public IActionResult Index(string gameType, string endDateInput, int precision = 0)
        {
            gameType ??= "tafeltennis";
            var endDate = endDateInput == null ? DateTime.MaxValue : DateTime.Parse(endDateInput);
            endDate = new DateTime(2020, 1, 1);
            
            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType, entry =>
            {
                var model = RankingViewModelsFor(gameType, DateTime.MinValue, endDate, precision)
                    .ToList();
                return model;
            });

            return View(cacheEntry);
        }
        
        [HttpGet("/rankings/{id}")]
        public IActionResult YearRanking(int id)
        {
            var gameType = "tafeltennis";
            if (id != 2019)
                id = 2019;

            var beginEnd = new DateTime(id, 1, 1);
            var endDate = new DateTime(id+1, 1, 1);
            
            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType + "-" + id, entry =>
            {
                var model = RankingViewModelsFor(gameType, beginEnd, endDate, 0)
                    .ToList();
                return model;
            });

            return View("Index", cacheEntry);
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

            return View("Index", cacheEntry);
        }



        [HttpGet("/rankings/month/{gametype}/{year}/{month}")]
        public IActionResult Month(string gameType, int year, int month)
        {
            gameType ??= "tafeltennis";
            var startDate = new DateTime(2019, 9, 1, 0, 0, 0);
            var endDate = new DateTime(2019, 10, 1, 0, 0, 0);

            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType + ":" + year + ":" + month, entry =>
            {
                var model = RankingViewModelsFor(gameType, startDate, endDate,  2).ToList();
                return model;
            });

            Response.Headers.Add("Refresh", "60");
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
            int minimalNumberOfGames = startDate.Year <= 2019 ? 7 : 0 ;
            var list = eloScores
                .Where((pair, i) => pair.Value.NumberOfGames >= minimalNumberOfGames)
                // TODO use id (guid) in stead of email address
                .Select(pair => new RankingViewModel
                {
                    EmailAddress = pair.Key.EmailAddress, 
                    NamePlayer = pair.Key.DisplayName, 
                    Points = pair.Value.EloScore.Round(precision), 
                    Ranking = ranking++,
                    TimeNumberOne = pair.Value.TimeNumberOne.ToString(@"%d")
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
    }
}