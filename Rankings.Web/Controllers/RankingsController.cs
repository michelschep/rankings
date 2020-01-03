using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
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

        [HttpGet("/rankings")]
        [HttpGet("/rankings/{gametype}/{endDateInput}")]
        public IActionResult Index(string gameType, string endDateInput, int precision = 0)
        {
            gameType ??= "tafeltennis";
            var endDate = endDateInput == null ? DateTime.MaxValue : DateTime.Parse(endDateInput);

            var cacheEntry = _memoryCache.GetOrCreate("ranking-" + gameType, entry =>
            {
                var model = RankingViewModelsFor(gameType, DateTime.MinValue, endDate, precision)
                    .ToList();
                return model;
            });

            return View(cacheEntry);
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
            var eloScores = _statisticsService.TheNewRanking(gameType, startDate, endDate);

            // Fill view model with elo score
            var list = eloScores
                // TODO use id (guid) in stead of email address
                .Select(pair => new RankingViewModel {EmailAddress = pair.Key.EmailAddress, NamePlayer = pair.Key.DisplayName, Points = pair.Value.EloScore.Round(precision), Ranking = pair.Value.Ranking})
                .ToList();

            foreach (var rankingViewModel in list)
            {
                rankingViewModel.History = _statisticsService.History(rankingViewModel.EmailAddress).ToList();
                rankingViewModel.WinPercentage = (100* _statisticsService.WinPercentage(rankingViewModel.EmailAddress)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.SetWinPercentage = (100* _statisticsService.SetWinPercentage(rankingViewModel.EmailAddress)).Round().ToString(CultureInfo.CurrentCulture);
                rankingViewModel.RecordWinningStreak = _statisticsService.RecordWinningStreak(rankingViewModel.EmailAddress);
                rankingViewModel.CurrentWinningStreak = _statisticsService.CurrentWinningStreak(rankingViewModel.EmailAddress);
                rankingViewModel.RecordEloStreak = (int) _statisticsService.RecordEloStreak(rankingViewModel.EmailAddress).Round();
                rankingViewModel.CurrentEloStreak = (int) _statisticsService.CurrentEloStreak(rankingViewModel.EmailAddress).Round();
            }

            return list;
        }

        [Obsolete("This one will be replaced by better mechnism")]
        private IEnumerable<RankingViewModel> ObsoleteRankingViewModels(string gameType, DateTime startDate,
            DateTime endDate, int numberOfGames)
        {
            var ratings = _statisticsService.Ranking(gameType, startDate, endDate);
            var ranking = 1;
            var lastPointInTime = _statisticsService.CalculateStats(startDate, endDate);

            var model = ratings.DeprecatedRatings.Where(pair => pair.Value.NumberOfGames >= numberOfGames)
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    NumberOfGames = r.Value.NumberOfGames,
                    WinPercentage = Math.Round(r.Value.WinPercentage, 0, MidpointRounding.AwayFromZero)
                        .ToString(CultureInfo.InvariantCulture),
                    SetWinPercentage = Math.Round(r.Value.SetWinPercentage, 0, MidpointRounding.AwayFromZero)
                        .ToString(CultureInfo.InvariantCulture),
                    Points = Math.Round(r.Value.Ranking, 0, MidpointRounding.AwayFromZero),
                    NamePlayer = r.Key.DisplayName,
                    Ranking = (ranking++),
                    History = ToHistory(r),
                    RecordWinningStreak = ToWinningStreak(r),
                    CurrentWinningStreak = ToCurrentWinningStreak(r),
                    RecordEloStreak = (int) r.Value.BestEloSeries,
                    CurrentEloStreak = (int) r.Value.CurrentEloSeries,
                    SkalpStreak = (int) r.Value.SkalpStreak,
                    Goat = (int) r.Value.Goat,
                    TimeNumberOne =
                        Math.Round(
                            (new TimeSpan(0, lastPointInTime.Value.NewPlayerStats[r.Key].TimeNumberOne, 0).TotalDays),
                            0,
                            MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture)
                });
            return model;
        }

        private int ToWinningStreak(in KeyValuePair<Profile, ObsoletePlayerStats> r)
        {
            return r.Value.History.Split('L').Select(s => s.Length).Max();
        }

        private int ToCurrentWinningStreak(in KeyValuePair<Profile, ObsoletePlayerStats> r)
        {
            return r.Value.History.Split('L').Select(s => s.Length).Last();
        }

        private static List<char> ToHistory(KeyValuePair<Profile, ObsoletePlayerStats> r)
        {
            var chars = r.Value.History.ToCharArray().Reverse().ToList();
            return chars.Take(7).Reverse().ToList();
        }
    }
}