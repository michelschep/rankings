using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;
using Rankings.Core.Models;
using Rankings.Core.Services;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class RankingsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMemoryCache _memoryCache;
        private readonly EloConfiguration _eloConfiguration;
        private readonly ILogger<RankingsController> _logger;

        public RankingsController(IStatisticsService rankingService, IMemoryCache memoryCache, EloConfiguration eloConfiguration, ILogger<RankingsController> logger)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _eloConfiguration = eloConfiguration ?? throw new ArgumentNullException(nameof(eloConfiguration));
            _logger = logger;
        }

        [HttpGet("/rankings/{year}")]
        public IActionResult YearRanking(int year)
        {
            _logger.LogError("Get ranking for year {Year}", year);
            
            ViewBag.Title = $"The {year} Ranking";
            ViewBag.Message = "";

            /*
            if (year == 2021)
            {
                ViewBag.Message += "De ranking is nog even niet zichtbaar maar (nog even geduld)...";
                ViewBag.Message += "<br>";
                ViewBag.Message += "<br>";
                ViewBag.Message += "<p style='color:red'>...INVOEREN VAN UITSLAGEN KAN NATUURLIJK AL GEWOON ZOALS GEBRUIKELIJK!<br>";
                ViewBag.Message += "DUS ALLE UITSLAGEN VAN 2021 TELLEN MEE!!!<p/>";
                ViewBag.Message += "<br>";
                ViewBag.Message += "<br>";
                ViewBag.Message += "<br>Aantal nieuwe features/regels/algoritmes/formules";
                ViewBag.Message += "<ol>";
                ViewBag.Message += "<li>Ranking zal zichtbaar zijn wanneer we minimaal een podium (top 3) hebben</li>";
                ViewBag.Message += "<li>Na 7 wedstrijden ben je zichtbaar in de ranking</li>";
                ViewBag.Message += "<li>Door te spelen tegen meer verschillende tegenstanders kun je harder stijgen (bij winst) en minder hard dalen (bij verlies)</li>";
                ViewBag.Message += "<li>Speel je gemiddeld tegen minder tegenstanders kun minder snel stijgen maar wel sneller dalen</li>";
                ViewBag.Message += "<li>Voor de echte tafeltennis nerds: ook set standen binnenkort in te voeren</li>";
                ViewBag.Message += "</ol>";
                ViewBag.Message += "<br>";
                ViewBag.Message += "<br>Voor de nerds: code wordt omgezet naar CQRS/event driven/event sourcing/aggregates/projections/process managers";

                return View("Index", new List<RankingViewModel>());
            }
*/
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

        private IEnumerable<RankingViewModel> RankingViewModelsFor(string gameType, DateTime startDate, DateTime endDate, int precision = 2)
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

            if (list.Count() >= 3)
                return list;
            
            return Enumerable.Empty<RankingViewModel>();
        }

        private int NumberOfGames(DateTime startDate)
        {
            if (startDate.Year == 2019)
                return 7;

            if (startDate.Year == 2020)
                return 7;
            
            if (startDate.Year == 2021)
                return 7;
            
            if (startDate.Year == 2022)
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