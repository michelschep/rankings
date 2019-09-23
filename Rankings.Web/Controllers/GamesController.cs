using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly IRankingService _rankingService;

        public GamesController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            // TODO fix loading entities
            var players = _rankingService.Profiles();
            var venues = _rankingService.GetVenues();
            var gameTypes = _rankingService.GameTypes();

            var games = _rankingService
                .Games()
                // TODO quick fix. Do not show same player games. 
                // TODO edit game is missing and needed.
                .Where(game => game.Player1.EmailAddress != game.Player2.EmailAddress)
                .OrderByDescending(game => game.RegistrationDate);
            var model = games.Select(type => new GameSummaryViewModel
            {
                Id = type.Id,
                GameType = type.GameType.DisplayName,
                Venue = type.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayer = type.Player1.DisplayName,
                NameSecondPlayer = type.Player2.DisplayName,
                RegistrationDate = type.RegistrationDate,
                ScoreFirstPlayer = type.Score1,
                ScoreSecondPlayer = type.Score2
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new CreateGameViewModel
            {
                Players = _rankingService.Profiles().OrderBy(profile => profile.DisplayName).Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress)),
                GameTypes = _rankingService.GameTypes().Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _rankingService.GetVenues().Select(type => new SelectListItem(type.DisplayName, type.Code))
            });
        }

        [HttpPost]
        public IActionResult Create(CreateGameViewModel model)
        {
            // TODO must be a better way. For now make it work and no per or mem issue.
            var players = _rankingService.Profiles().ToList();
            var gameTypes = _rankingService.GameTypes().ToList();
            var venues = _rankingService.GetVenues().ToList();

            var game = new Game
            {
                GameType = gameTypes.Single(type => type.Code == model.GameType),
                Venue = venues.Single(profile => profile.Code == model.Venue),
                Player1 = players.Single(profile => profile.EmailAddress == model.NameFirstPlayer),
                Player2 = players.Single(profile => profile.EmailAddress == model.NameSecondPlayer),
                Score1 = model.ScoreFirstPlayer,
                Score2 = model.ScoreSecondPlayer,
            };

            _rankingService.RegisterGame(game);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _rankingService.DeleteGame(id);
            return RedirectToAction("Index");
        }
    }
}