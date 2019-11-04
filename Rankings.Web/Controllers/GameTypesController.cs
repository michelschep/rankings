using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class GameTypesController : Controller
    {
        private readonly IGamesService _gamesService;

        public GameTypesController(IGamesService gamesService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        public IActionResult Index()
        {
            var gameTypes = _gamesService.GameTypes();
            var model = gameTypes.Select(type => new GameTypeViewModel
            {
                Code = type.Code,
                DisplayName = type.DisplayName
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GameTypeViewModel viewModel)
        {
            _gamesService.CreateGameType(new GameType
            {
                Code = viewModel.Code,
                DisplayName = viewModel.DisplayName
            });

            return RedirectToAction("Index");
        }
    }
}