﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
                Headers = new List<string>() {"Total Elo", "Penalty", "Diff", "Time in hours", "Avg Elo", "Elo", "Elo/h"},
                Values = result.Select(pair =>
                {
                    var viewItem = new ViewItem {Index = (index++).ToString(), Name = pair.Key.DisplayName,};
                    viewItem.Scores.Add(pair.Value["total elo"].Round().ToString());
                    viewItem.Scores.Add(pair.Value["penalty"].Round().ToString());
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

                    viewItem.Scores.Add(pair.Value["avg elo"].Round().ToString());
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