﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            
            var model = ratings.DeprecatedRatings.Where(pair => pair.Value.NumberOfGames >= 6)
                .OrderByDescending(pair => pair.Value.Ranking)
                .Select(r => new RankingViewModel
                {
                    WinPercentage = Math.Round(r.Value.WinPercentage,0,MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture),
                    SetWinPercentage = Math.Round(r.Value.SetWinPercentage, 0, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture),
                    Points = Math.Round(r.Value.Ranking,0,MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture), 
                    NamePlayer = r.Key.DisplayName, 
                    Ranking = (ranking++) + ".",
                    History = ToHistory(r)
                });

            return View(model);
        }

        private static List<char> ToHistory(KeyValuePair<Profile, PlayerStats> r)
        {
            return r.Value.History.ToCharArray().Reverse().ToList().Take(7).Reverse().ToList();
        }
    }
}