using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class GameTypesController : Controller
    {
        private readonly IGamesProjection _gamesProjection;

        public GameTypesController(IGamesProjection gamesProjection)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
        }

        public IActionResult Index()
        {
            var model = _gamesProjection.List(new AllGameTypes())
                .Select(type => new GameTypeViewModel
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
            _gamesProjection.CreateGameType(new GameType
            {
                Code = viewModel.Code,
                DisplayName = viewModel.DisplayName
            });

            return RedirectToAction("Index");
        }
    }
}