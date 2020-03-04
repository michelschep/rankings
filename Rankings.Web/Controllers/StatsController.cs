using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;

namespace Rankings.Web.Controllers
{
    public class StatsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatsController(IStatisticsService rankingService)
        {
            _statisticsService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        [HttpGet("/stats/average-elo")]
        public IActionResult AverageElo(int year)
        {
            var result = _statisticsService
                .TotalElo(GameTypes.TableTennis, new DateTime(year, 1, 1), new DateTime(year, 12, 31))
                .OrderByDescending(pair => pair.Value["avg elo"])
                .ToDictionary(pair => pair.Key, pair => pair.Value["avg elo"])
                .ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = "Average Elo Score " + year;
            return View("Index", viewModel);
        }

        [HttpGet("/stats/duel-league")]
        public IActionResult DuelLeague()
        {
            var result = _statisticsService.GameSummaries(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31)).ToList();
            var map = result
                .SelectMany(summary => new[] {summary.Player1, summary.Player2})
                .Distinct()
                .ToDictionary(s => s, s => new LeagueScore(s));

            foreach (var gameSummary in result)
            {
                map[gameSummary.Player1].Played += 1;
                map[gameSummary.Player2].Played += 1;
                map[gameSummary.Player1].Percentage1 += gameSummary.PercentageScore1;
                map[gameSummary.Player1].Percentage2 += gameSummary.PercentageScore2;
                map[gameSummary.Player2].Percentage1 += gameSummary.PercentageScore2;
                map[gameSummary.Player2].Percentage2 += gameSummary.PercentageScore1;

                var max = 1; //gameSummary.Score1 + gameSummary.Score2 >= 5 ? 5 : gameSummary.Score1 + gameSummary.Score2;

                if (gameSummary.Score1 > gameSummary.Score2)
                {
                    map[gameSummary.Player1].Score += 3 * max;
                    map[gameSummary.Player1].Won += 1;
                    map[gameSummary.Player2].Lost += 1;
                }

                if (gameSummary.Score1 < gameSummary.Score2)
                {
                    map[gameSummary.Player2].Score += 3 * max;
                    map[gameSummary.Player2].Won += 1;
                    map[gameSummary.Player1].Lost += 1;
                }

                if (gameSummary.Score1 == gameSummary.Score2)
                {
                    map[gameSummary.Player1].Score += max;
                    map[gameSummary.Player2].Score += max;
                    map[gameSummary.Player1].Draw += 1;
                    map[gameSummary.Player2].Draw += 1;
                }
            }

            var ranking = 1;
            foreach (var leagueScore in map.OrderByDescending(pair => pair.Value.Score))
            {
                leagueScore.Value.Percentage1 /= leagueScore.Value.Played;
                leagueScore.Value.Percentage2 /= leagueScore.Value.Played;
                leagueScore.Value.Ranking = ranking++;
            }

            return View(map.Select(pair => pair.Value).OrderByDescending(score => score.Score));
        }

        [HttpGet("/stats/{year}")]
        public IActionResult Index(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);
            var result = _statisticsService.FibonacciScore(startDate, endDate).Where((pair, i) => pair.Value > 0).OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem()
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = $"Fibonacci Ranking {year}";
            return View(viewModel);
        }

        [HttpGet("/stats/winning-streaks-summaries")]
        public IActionResult WinningStreaksSummaries()
        {
            var winningStreaks = _statisticsService
                .WinningStreaks(DateTime.MinValue, DateTime.MaxValue)
                .GroupBy(streak => streak.Player)
                .Select(grouping => new
                {
                    Profile = grouping.Key,
                    AverageStreak = (decimal) grouping.Sum(streak => streak.NumberOfGames) / grouping.Count()
                }).OrderByDescending(arg => arg.AverageStreak);

            var losingStreaks = _statisticsService
                .LosingStreaks(DateTime.MinValue, DateTime.MaxValue)
                .GroupBy(streak => streak.Player)
                .Select(grouping => new
                {
                    Profile = grouping.Key,
                    AverageStreak = (decimal) grouping.Sum(streak => streak.NumberOfGames) / grouping.Count()
                }).OrderBy(arg => arg.AverageStreak);


            var winningStreaksViewModel = new WinningStreaksSummaryViewModel
            {
                Items = winningStreaks.Select(arg => new StreakSummaryViewModel
                {
                    Player = arg.Profile.DisplayName,
                    AverageWinningStreak = arg.AverageStreak.Round(1).ToString()
                })
            };

            return View(winningStreaksViewModel);
        }

        [HttpGet("/stats/losing-streaks-summaries")]
        public IActionResult LosingStreaksSummaries()
        {
            var losingStreaks = _statisticsService
                .LosingStreaks(DateTime.MinValue, DateTime.MaxValue)
                .GroupBy(streak => streak.Player)
                .Select(grouping => new
                {
                    Profile = grouping.Key,
                    AverageStreak = (decimal) grouping.Sum(streak => streak.NumberOfGames) / grouping.Count()
                }).OrderBy(arg => arg.AverageStreak);

            var winningStreaksViewModel = new WinningStreaksSummaryViewModel
            {
                Items = losingStreaks.Select(arg => new StreakSummaryViewModel
                {
                    Player = arg.Profile.DisplayName,
                    AverageWinningStreak = arg.AverageStreak.Round(1).ToString()
                })
            };

            return View("WinningStreaksSummaries", winningStreaksViewModel);
        }

        [HttpGet("/stats/winning-streaks")]
        public IActionResult WinningStreaks()
        {
            var streaks = _statisticsService
                .WinningStreaks(DateTime.MinValue, DateTime.MaxValue)
                .Where((streak, i) => streak.NumberOfGames >= 5)
                .OrderByDescending(streak => streak.NumberOfGames * streak.AverageElo)
                .ThenByDescending(streak => streak.NumberOfGames)
                .ThenByDescending(streak => streak.AverageElo);

            var streakViewModels = streaks.Select(streak => new StreakViewModel
            {
                NumberOfGames = streak.NumberOfGames.ToString(),
                EndDate = streak.EndDate.ToString("yyyy/MM/dd H:mm"),
                Player = streak.Player.DisplayName,
                StartDate = streak.StartDate.ToString("yyyy/MM/dd H:mm"),
                NumberOfDays = Math.Round((streak.EndDate - streak.StartDate).TotalDays, 0),
                AverageElo = streak.AverageElo.Round().ToString(),
                Volume = (streak.AverageElo * streak.NumberOfGames).Round().ToString()
            });

            var winningStreaksViewModel = new WinningStreaksViewModel
            {
                Streaks = streakViewModels
            };

            return View(winningStreaksViewModel);
        }

        [HttpGet("/stats/goat/{year}")]
        public IActionResult Goat(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);
            var result = _statisticsService.GoatScore(startDate, endDate).OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem()
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = $"Goat Ranking {year}";
            return View("Index", viewModel);
        }

        [HttpGet("/stats/duels")]
        [HttpGet("/stats/duels/{year}")]
        public IActionResult Duels(int? year)
        {
            var startDate = year.HasValue ? new DateTime(year.Value, 1, 1) : DateTime.MinValue;
            var endDate = year.HasValue ? new DateTime(year.Value, 12, 31) : DateTime.MaxValue;

            var gameSummaries = _statisticsService.GameSummaries(startDate, endDate)
                .OrderByDescending(summary => summary.TotalGames)
                .ThenByDescending(summary => Math.Abs(summary.PercentageSet1 - summary.PercentageSet2));

            return View("Duels", gameSummaries);
        }

        [HttpGet("/stats/strength")]
        public IActionResult Strength()
        {
            var result = _statisticsService.StrengthGamesPerPlayer(DateTime.MinValue, DateTime.MaxValue)
                .OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = "Strength Ranking";
            return View("Index", viewModel);
        }

        [HttpGet("/stats/oponents")]
        public IActionResult StrengthOponents()
        {
            var result = _statisticsService.StrengthOponentPerPlayer(DateTime.MinValue, DateTime.MaxValue)
                .OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = "Strength Oponents Ranking";
            return View("Index", viewModel);
        }

        [HttpGet("/stats/wins")]
        public IActionResult StrengthWins()
        {
            var result = _statisticsService.StrengthWinsPerPlayer(DateTime.MinValue, DateTime.MaxValue)
                .OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = "Strength Wins Ranking";
            return View("Index", viewModel);
        }

        [HttpGet("/stats/losts")]
        public IActionResult StrengthLosts()
        {
            var result = _statisticsService.StrengthLostsPerPlayer(DateTime.MinValue, DateTime.MaxValue)
                .OrderByDescending(pair => pair.Value).ToList();

            var index = 1;
            var viewModel = result.Select(pair => new RankingViewItem
            {
                Index = (index++).ToString(),
                Name = pair.Key.DisplayName,
                Score = pair.Value.Round().ToString()
            });

            ViewBag.Title = "Strength Losts Ranking";
            return View("Index", viewModel);
        }

        [HttpGet("/rankings/playeroftheyear")]
        public IActionResult PlayerOfTheYear()
        {
            var result = _statisticsService
                .TotalElo(GameTypes.TableTennis, new DateTime(2020, 1, 1), new DateTime(2020, 12, 31))
                .OrderByDescending(pair => pair.Value.First().Value).ToList();

            var maxEloTotalScore = result.Max(pair => pair.Value["total elo"]).Round();
            var speedNumberOne = result.First().Value["elo/h"];

            var index = 1;
            var viewModel = new ViewItems
            {
                Headers = new List<string>() {"Total Elo", "Diff", "Behind in hours", "Elo/day", "Elo/hour"},
                Values = result.Select(pair =>
                {
                    var viewItem = new ViewItem {Index = (index++).ToString(), Name = pair.Key.DisplayName,};
                    viewItem.Scores.Add(pair.Value["total elo"].Round().ToString());
                    var diffScore = (pair.Value["total elo"].Round() - maxEloTotalScore);
                    viewItem.Scores.Add(diffScore.ToString());

                    var thisSpeed = pair.Value["elo/h"].Round();
                    if (thisSpeed > speedNumberOne)
                    {
                        var timeNeeded = -1 * diffScore / (thisSpeed - speedNumberOne);
                        viewItem.Scores.Add(timeNeeded.Round().ToString());
                    }
                    else
                    {
                        viewItem.Scores.Add("");
                    }

                    //viewItem.Scores.Add(pair.Value["avg elo"].Round().ToString());
                    viewItem.Scores.Add(pair.Value["current elo"].Round().ToString());

                    viewItem.Scores.Add(pair.Value["elo/h"].Round().ToString());
                    return viewItem;
                })
            };
            ViewBag.Title = "Player of the Year";
            return View("MultiValues", viewModel);
        }

        [HttpGet("/Profiles/Details/Stats/PlayerHeatmapData/{profile}")]
        public IActionResult PlayerHeatmapData(string profile)
        {
            var statGames = _statisticsService
                .EloGames(profile).ToList();
            //var result = statGames
            //    .GroupBy(game => new {Month = game.RegistrationDate.Month, Variable = 50 * (int)(game.EloPlayer2 / 50)})
            //    .Select(game => new Item {Group = game.Key.Month.ToString(), Variable = game.Key.Variable.ToString(), Value = (int)game.Sum(statGame => statGame.Delta1).Round()})
            //    .ToList();

            var result = statGames
                .GroupBy(game => new {Month = game.RegistrationDate.Month, Variable = 50 * (int) (game.EloPlayer2 / 50)})
                .Select(game => new Item {Group = game.Key.Month.ToString(), Variable = game.Key.Variable.ToString(), Value = (int) 100 * game.Count(statGame => statGame.Score1 > statGame.Score2) / game.Count()})
                .ToList();
            return new JsonResult(result);
        }

        private int DetermineVariable(decimal gameEloPlayer)
        {
            return 100 * (int) (gameEloPlayer / 100);
        }

        public class Item
        {
            public string Group { get; set; }
            public string Variable { get; set; }
            public int Value { get; set; }
        }
    }

    public class LeagueScore
    {
        public int Ranking { get; set; }
        public string EmailAdress { get; }
        public int Score { get; set; }
        public int Draw { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public int Played { get; set; }
        public int Percentage1 { get; set; }
        public int Percentage2 { get; set; }

        public LeagueScore(string emailAdress)
        {
            EmailAdress = emailAdress;
        }
    }

    public class StreakSummaryViewModel
    {
        public string Player { get; set; }
        public string AverageWinningStreak { get; set; }
        public string AverageLosingStreak { get; set; }
    }

    public class WinningStreaksSummaryViewModel
    {
        public IEnumerable<StreakSummaryViewModel> Items { get; set; }
    }

    public class StreakViewModel
    {
        public string Player { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Details { get; set; }
        public string StreakTerminator { get; set; }
        public string NumberOfGames { get; set; }
        public string NumberOfEloPints { get; set; }
        public string AverageElo { get; set; }
        public string Volume { get; set; }
        public double NumberOfDays { get; set; }
    }

    public class WinningStreaksViewModel
    {
        public IEnumerable<StreakViewModel> Streaks { get; set; }
    }

    public class ViewItems
    {
        public List<string> Headers { get; set; }
        public IEnumerable<ViewItem> Values { get; set; }
    }

    public class ViewItem
    {
        public ViewItem()
        {
            Scores = new List<string>();
        }

        public string Index { get; set; }
        public string Name { get; set; }
        public List<string> Scores { get; set; }
    }

    public class RankingViewItem
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
    }
}