using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesService _gamesService;

        public GamesController(IGamesService gamesService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        public IActionResult Index()
        {
            var model = CreateGameSummaryViewModels();
            Response.Headers.Add("Refresh", "30");
            return View(model);
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AdminIndex()
        {
            var model = CreateGameSummaryViewModels();

            return View(model);
        }

        private List<GameSummaryViewModel> CreateGameSummaryViewModels()
        {
            // TODO fix loading entities
            var players = _gamesService.Profiles();
            var venues = _gamesService.GetVenues();
            var gameTypes = _gamesService.GameTypes();

            var games = _gamesService
                .Games()
                // TODO quick fix. Do not show same player games. 
                // TODO edit game is missing and needed.
                .Where(game => game.Player1.EmailAddress != game.Player2.EmailAddress)
                .Where(game => game.RegistrationDate > DateTime.Now.AddHours(-72))
                .OrderByDescending(game => game.RegistrationDate);
            var model = games.Select(type => new GameSummaryViewModel
            {
                Id = type.Id,
                GameType = type.GameType.DisplayName,
                Venue = type.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayer = type.Player1.DisplayName,
                NameSecondPlayer = type.Player2.DisplayName,
                RegistrationDate = type.RegistrationDate.AddHours(2).ToString("yyyy/MM/dd H:mm"),
                ScoreFirstPlayer = type.Score1,
                ScoreSecondPlayer = type.Score2,
                IsEditable =
                    (type.Player1.EmailAddress == User.Identity.Name || type.Player2.EmailAddress == User.Identity.Name)
                    && type.RegistrationDate > DateTime.Now.AddHours(-24)
            }).ToList();
            return model;
        }

        public IActionResult Create()
        {
            var profiles = _gamesService.Profiles().ToList();
            var currentProfile = profiles.Single(profile => profile.EmailAddress == User.Identity.Name);
            var oponentPlayers = profiles
                .Where(profile => profile.EmailAddress != User.Identity.Name)
                .OrderBy(profile => profile.DisplayName)
                .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));

            return View(new CreateGameViewModel
            {
                NameFirstPlayer = User.Identity.Name,
                Players = new SelectListItem[1] {new SelectListItem(currentProfile.DisplayName, currentProfile.EmailAddress) },
                OpponentPlayers = oponentPlayers,
                GameTypes = _gamesService.GameTypes().Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesService.GetVenues().Select(type => new SelectListItem(type.DisplayName, type.Code))
            });
        }

        [HttpPost]
        public IActionResult Create(CreateGameViewModel model)
        {
            // TODO must be a better way. For now make it work and no per or mem issue.
            var players = _gamesService.Profiles().ToList();
            var gameTypes = _gamesService.GameTypes().ToList();
            var venues = _gamesService.GetVenues().ToList();

            var game = new Game
            {
                GameType = gameTypes.Single(type => type.Code == model.GameType),
                Venue = venues.Single(profile => profile.Code == model.Venue),
                Player1 = players.Single(profile => profile.EmailAddress == model.NameFirstPlayer),
                Player2 = players.Single(profile => profile.EmailAddress == model.NameSecondPlayer),
                Score1 = model.ScoreFirstPlayer,
                Score2 = model.ScoreSecondPlayer,
            };

            _gamesService.RegisterGame(game);
            return RedirectToAction("Index", "Rankings");
        }

        public IActionResult Edit(int id)
        {
            var game = _gamesService.Games().Single(g => g.Id == id);

            if (game.RegistrationDate < DateTime.Now.AddHours(-24))
                return RedirectToAction("Index", "Rankings");

            if (game.Player1.EmailAddress != User.Identity.Name &&
                game.Player2.EmailAddress != User.Identity.Name)
                return RedirectToAction("Index", "Rankings");

            var viewModel = new CreateGameViewModel
            {
                Players = _gamesService.Profiles().OrderBy(profile => profile.DisplayName).Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress)),
                GameTypes = _gamesService.GameTypes().Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesService.GetVenues().Select(type => new SelectListItem(type.DisplayName, type.Code)),

                Id = game.Id,
                RegistrationDate = game.RegistrationDate,
                NameFirstPlayer = game.Player1.EmailAddress,
                NameSecondPlayer = game.Player2.EmailAddress,
                GameType = game.GameType.Code,
                ScoreFirstPlayer = game.Score1,
                ScoreSecondPlayer = game.Score2,
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(CreateGameViewModel model)
        {
            // TODO must be a better way. For now make it work and no per or mem issue.
            var venues = _gamesService.GetVenues().ToList();
            var gameTypes = _gamesService.GameTypes().ToList();

            var game = _gamesService.Games().Single(g => g.Id == model.Id);

            //game.RegistrationDate = model.RegistrationDate;
            game.Venue =venues.Single(profile => profile.Code == model.Venue);
            game.GameType = gameTypes.Single(type => type.Code == model.GameType);
            game.Score1 = model.ScoreFirstPlayer;
            game.Score2 = model.ScoreSecondPlayer;

            _gamesService.Save(game);

            return RedirectToAction("Index", "Rankings");
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int id)
        {
            _gamesService.DeleteGame(id);
            return RedirectToAction("Index", "Rankings");
        }
    }
}