﻿using System;
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
    }

    public class RankingViewItem
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
    }
}