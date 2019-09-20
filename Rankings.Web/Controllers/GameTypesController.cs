using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class GameTypesController : Controller
    {
        private readonly IRankingService _rankingService;

        public GameTypesController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            var gameTypes = _rankingService.GameTypes();
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
            _rankingService.CreateGameType(new GameType
            {
                Code = viewModel.Code,
                DisplayName = viewModel.DisplayName
            });

            return RedirectToAction("Index");
        }
    }
}