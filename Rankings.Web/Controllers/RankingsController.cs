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
        private readonly EloConfiguration _eloConfiguration;

        public RankingsController(IStatisticsService rankingService, IMemoryCache memoryCache, EloConfiguration eloConfiguration)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _eloConfiguration = eloConfiguration ?? throw new ArgumentNullException(nameof(eloConfiguration));
        }

        [HttpGet("/rankings/{year}")]
        public IActionResult YearRanking(int year)
        {
            ViewBag.Title = $"The {year} Ranking";
            ViewBag.Message = "";

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
        [HttpGet("/rankings/eternal/{precision}")]
        public IActionResult EternalRanking(int? precision)
        {
            var gameType = "tafeltennis";
            var beginEnd = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType + "-eternal", entry =>
            {
                var model = RankingViewModelsFor(gameType, beginEnd, endDate, precision ?? 0)
                    .ToList();
                return model;
            });

            this.ViewBag.Title = "The Eternal Ranking";
            this.ViewBag.Message = "";

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
            int minimalNumberOfGames = NumberOfGames(startDate);

            var list = eloScores
                .Where((pair, i) => pair.Value.NumberOfGames >= minimalNumberOfGames)
                // TODO use id (guid) in stead of email address
                .Select(pair => new RankingViewModel
                {
                    EmailAddress = pair.Key.EmailAddress,
                    NamePlayer = pair.Key.DisplayName,
                    Points = pair.Value.EloScore.Round(precision),
                    Ranking = CreateIcon(ranking++, _eloConfiguration),
                    TimeNumberOne = pair.Value.TimeNumberOne > new TimeSpan(0) ? pair.Value.TimeNumberOne.ToString(@"d\.hh\:mm") : ""
                })
                .ToList();

            foreach (var rankingViewModel in list)
            {
                rankingViewModel.History = _statisticsService.History(rankingViewModel.EmailAddress, startDate, endDate).ToList();
                rankingViewModel.WinPercentage = (100 * _statisticsService.WinPercentage(rankingViewModel.EmailAddress, startDate, endDate)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.SetWinPercentage = (100 * _statisticsService.SetWinPercentage(rankingViewModel.EmailAddress, startDate, endDate)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.RecordWinningStreak = _statisticsService.RecordWinningStreak(rankingViewModel.EmailAddress, startDate, endDate);
                rankingViewModel.CurrentWinningStreak = _statisticsService.CurrentWinningStreak(rankingViewModel.EmailAddress, startDate, endDate);
                rankingViewModel.RecordEloStreak = (int) _statisticsService.RecordEloStreak(rankingViewModel.EmailAddress, startDate, endDate).Round();
                rankingViewModel.CurrentEloStreak = (int) _statisticsService.CurrentEloStreak(rankingViewModel.EmailAddress, startDate, endDate).Round();
            }

            return list;
        }

        private int NumberOfGames(DateTime startDate)
        {
            if (startDate.Year == 2020)
                return 1;

            if (_eloConfiguration.NumberOfGames.HasValue)
                return _eloConfiguration.NumberOfGames.Value;

            return 7;
        }

        private string CreateIcon(int ranking, EloConfiguration eloConfiguration)
        {
            if (eloConfiguration.JustNumbersForRanking)
                return ranking.ToString();

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