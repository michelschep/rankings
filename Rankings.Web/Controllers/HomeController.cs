using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMemoryCache _memoryCache;

        public HomeController(IStatisticsService rankingService, IMemoryCache memoryCache)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet("/")]
        [HttpGet("/rankings")]
        public IActionResult Index()
        {
            var mainStats = _memoryCache.GetOrCreate("homepage", entry => CreateViewModel());

            return View(mainStats);
        }

        private MainStats CreateViewModel()
        {
            var mainStats = new MainStats();

            // *************** Eternal *************************
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;
            mainStats.Eternal = new List<Summary>
            {
                Top3Elo("Vitas Eternal Ranking", startDate, endDate),
                Top3TimeNumberOne("Vitas Eternal Time Number One", startDate, endDate),
                Top3GoatScores("Vitas Eternal Goat", startDate, endDate),
            };

            // *************** 2020 *************************
            startDate = new DateTime(2019, 1, 1);
            endDate = new DateTime(2019, 12, 31);
            mainStats.HallOfFame = new List<Summary>
            {
                Top3Elo("Vitas 2019 Ranking", startDate, endDate),
                Top3TimeNumberOne("Vitas 2019 Time Number One", startDate, endDate),
                Top3RecordWinningStreak("Best Winning Streak 2019", startDate, endDate),
                Top3RecordEloStreak("Best Elo Streak 2019", startDate, endDate),
                Top3Fibonacci("Fibonacci 2019", startDate, endDate)
            };

            // *************** 2020 *************************
            startDate = new DateTime(2020, 1, 1);
            endDate = new DateTime(2020, 12, 31);
            mainStats.RunningBattles = new List<Summary>
            {
                Top3Elo("Vitas 2020 Ranking", startDate, endDate),
                Top3TimeNumberOne("Vitas 2020 Time Number One", startDate, endDate),
                PlayerOfTheYear("Average Elo Score", startDate, endDate),
                Top3GoatScores("Goat 2020", startDate, endDate),
                Top3RecordWinningStreak("Best Winning Streak 2020", startDate, endDate),
                Top3RecordEloStreak("Best Elo Streak 2020", startDate, endDate),
                Top3Fibonacci("Fibonacci 2020", startDate, endDate)
            };

            mainStats.GameSummaries = _statisticsService.GameSummaries(new DateTime(2020, 1, 1), DateTime.MaxValue)
                .OrderByDescending(summary => summary.Score1 + summary.Score2)
                .Take(10);

            return mainStats;
        }

        private Summary Top3TimeNumberOne(string title, DateTime startDate, DateTime endDate)
        {
            var ranking = _statisticsService.Ranking(GameTypes.TableTennis, startDate, endDate);
            var list = new Dictionary<Profile, EloStatsPlayer>(ranking
                .OrderByDescending(pair => pair.Value.TimeNumberOne)
                .ThenByDescending(pair => pair.Value.EloScore)).ToList();

            return CreateSummary(list, i => i.TimeNumberOne.ToString(@"d\.hh\:mm"), title);
        }

        private Summary PlayerOfTheYear(string title, DateTime startDate, DateTime endDate)
        {
            var result = _statisticsService
                .TotalElo(GameTypes.TableTennis, new DateTime(2020, 1, 1), new DateTime(2020, 12, 31))
                .OrderByDescending(pair => pair.Value["avg elo"])
                .ToDictionary(pair => pair.Key, pair => pair.Value["avg elo"])
                .ToList();

            return CreateSummary<decimal>(result, i => i.Round().ToString(), title);
        }

        private Summary Top3Elo(string title, DateTime startDate, DateTime endDate)
        {
            var ranking = _statisticsService.Ranking("tafeltennis", startDate, endDate);
            var list = new Dictionary<Profile, EloStatsPlayer>(ranking.OrderByDescending(pair => pair.Value.EloScore)).ToList();

            return CreateSummary(list, i => i.EloScore.Round().ToString(), title);
        }

        private Summary Top3GoatScores(string title, DateTime startDate, DateTime endDate)
        {
            var goatScores = _statisticsService.GoatScore(startDate, endDate).OrderByDescending(pair => pair.Value).ToList();
            return CreateSummary(goatScores, i => i.Round().ToString(), title);
        }

        private Summary Top3Fibonacci(string title, DateTime startDate, DateTime endDate)
        {
            var winningStreaks = _statisticsService.FibonacciScore(startDate, endDate).OrderByDescending(pair => pair.Value).ToList();
            return CreateSummary(winningStreaks, i => i.Round().ToString(), title);
        }

        private Summary Top3RecordWinningStreak(string title, DateTime startDate, DateTime endDate)
        {
            var winningStreaks = _statisticsService.RecordWinningStreak(startDate, endDate).OrderByDescending(pair => pair.Value).ToList();
            return CreateSummary(winningStreaks, i => i.ToString(), title);
        }

        private Summary Top3RecordEloStreak(string title, DateTime startDate, DateTime endDate)
        {
            var winningStreaks = _statisticsService.RecordEloStreak(startDate, endDate).OrderByDescending(pair => pair.Value).ToList();
            return CreateSummary(winningStreaks, i => i.Round().ToString(), title);
        }

        private static Summary CreateSummary<T>(List<KeyValuePair<Profile, T>> winningStreaks, Func<T, string> toString, string title)
        {
            var numberOne = winningStreaks.First();
            var numberTwo = winningStreaks.Skip(1).First();
            var numberThree = winningStreaks.Skip(2).First();

            return new Summary
            {
                Title = title,
                Name1 = numberOne.Key.DisplayName, Score1 = toString(numberOne.Value),
                Name2 = numberTwo.Key.DisplayName, Score2 = toString(numberTwo.Value),
                Name3 = numberThree.Key.DisplayName, Score3 = toString(numberThree.Value),
            };
        }

        [HttpGet("/home/matrix")]
        [HttpGet("/home/matrix/{elo}")]
        public IActionResult Matrix(int? elo)
        {
            var model = new List<EloMatrixViewModel>();

            // TODO make it work for other types as well
            // TODO maybe better some build in types if it really means something different. Or constants to avoid strings all over the place.
            var ratings = _statisticsService
                .Ranking("tafeltennis", new DateTime(2020, 1, 1), DateTime.MaxValue);

            ratings.Add(new Profile() {DisplayName = "800"}, new EloStatsPlayer() {EloScore = 800});
            ratings.Add(new Profile() {DisplayName = "900"}, new EloStatsPlayer() {EloScore = 900});
            ratings.Add(new Profile() {DisplayName = "1000"}, new EloStatsPlayer() {EloScore = 1000});
            ratings.Add(new Profile() {DisplayName = "1100"}, new EloStatsPlayer() {EloScore = 1100});
            ratings.Add(new Profile() {DisplayName = "1200"}, new EloStatsPlayer() {EloScore = 1200});
            ratings.Add(new Profile() {DisplayName = "1300"}, new EloStatsPlayer() {EloScore = 1300});
            ratings.Add(new Profile() {DisplayName = "1400"}, new EloStatsPlayer() {EloScore = 1400});
            ratings.Add(new Profile() {DisplayName = "1500"}, new EloStatsPlayer() {EloScore = 1500});
            ratings.Add(new Profile() {DisplayName = "1600"}, new EloStatsPlayer() {EloScore = 1600});
            ratings.Add(new Profile() {DisplayName = "1700"}, new EloStatsPlayer() {EloScore = 1700});
            ratings.Add(new Profile() {DisplayName = "1800"}, new EloStatsPlayer() {EloScore = 1800});
            ratings.Add(new Profile() {DisplayName = "1900"}, new EloStatsPlayer() {EloScore = 1900});
            ratings.Add(new Profile() {DisplayName = "2000"}, new EloStatsPlayer() {EloScore = 2000});

            IEqualityComparer<KeyValuePair<Profile, EloStatsPlayer>> comparer = new MatrixComparer();
            ratings = new Dictionary<Profile, EloStatsPlayer>(ratings.Distinct(comparer));

            var thisPlayerElo = elo ?? ratings.First(pair => pair.Key.EmailAddress == User.Identity.Name).Value.EloScore;

            foreach (var stats in ratings.OrderByDescending(pair => pair.Value.EloScore))
            {
                var eloDiff = (int) Math.Round(stats.Value.EloScore - thisPlayerElo, 0, MidpointRounding.AwayFromZero);
                var oneOneDraw = CalculateDeltaResult(thisPlayerElo, 1, 1, stats.Value.EloScore);
                var twoToDraw = CalculateDeltaResult(thisPlayerElo, 2, 2, stats.Value.EloScore);
                var oneZeroWin = CalculateDeltaResult(thisPlayerElo, 1, 0, stats.Value.EloScore);
                var oneZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 1, stats.Value.EloScore);

                var twoZeroWin = CalculateDeltaResult(thisPlayerElo, 2, 0, stats.Value.EloScore);
                var twoOneWin = CalculateDeltaResult(thisPlayerElo, 2, 1, stats.Value.EloScore);
                var twoZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 2, stats.Value.EloScore);
                var twoOneLost = CalculateDeltaResult(thisPlayerElo, 1, 2, stats.Value.EloScore);

                var threeZeroWin = CalculateDeltaResult(thisPlayerElo, 3, 0, stats.Value.EloScore);
                var threeOneWin = CalculateDeltaResult(thisPlayerElo, 3, 1, stats.Value.EloScore);
                var threeTwoWin = CalculateDeltaResult(thisPlayerElo, 3, 2, stats.Value.EloScore);

                var threeZeroLost = CalculateDeltaResult(thisPlayerElo, 0, 3, stats.Value.EloScore);
                var threeOneLost = CalculateDeltaResult(thisPlayerElo, 1, 3, stats.Value.EloScore);
                var threeTwoLost = CalculateDeltaResult(thisPlayerElo, 2, 3, stats.Value.EloScore);

                var line = new EloMatrixViewModel(stats.Key.DisplayName, (int) Math.Round(stats.Value.EloScore, 0, MidpointRounding.AwayFromZero), eloDiff, oneOneDraw,
                    oneZeroWin, oneZeroLost, twoZeroWin, twoZeroLost, threeZeroWin,
                    threeZeroLost)
                {
                    ThreeOneWin = threeOneWin,
                    ThreeTwoWin = threeTwoWin,
                    ThreeOneLost = threeOneLost,
                    ThreeTwoLost = threeTwoLost,
                    TwoOneLost = twoOneLost,
                    TwoOneWin = twoOneWin,
                    TwoTwoDraw = twoToDraw
                };
                model.Add(line);
            }

            return View(model);
        }

        private decimal CalculateDeltaResult(decimal thisPlayerElo, int gameScore1, int gameScore2, decimal eloScoreOponent)
        {
            var eloCalculator = new EloCalculatorVersion2020();
            var deltaFirstPlayer = eloCalculator.CalculateDeltaPlayer(thisPlayerElo, eloScoreOponent, gameScore1, gameScore2);
            return deltaFirstPlayer.Round(2);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }

    public class Summary
    {
        public string Title { get; set; }
        public string Name1 { get; set; }
        public string Score1 { get; set; }
        public string Name2 { get; set; }
        public string Score2 { get; set; }
        public string Name3 { get; set; }
        public string Score3 { get; set; }
    }

    public class MatrixComparer : IEqualityComparer<KeyValuePair<Profile, EloStatsPlayer>>
    {
        public bool Equals(KeyValuePair<Profile, EloStatsPlayer> x, KeyValuePair<Profile, EloStatsPlayer> y)
        {
            return x.Value.EloScore == y.Value.EloScore;
        }

        public int GetHashCode(KeyValuePair<Profile, EloStatsPlayer> obj)
        {
            return obj.Value.EloScore.GetHashCode();
        }
    }
}