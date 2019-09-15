using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            return View(gameTypes.Select(type => new GameTypeViewModel
            {
                Code = type.Code,
                DisplayName = type.DisplayName
            }));
        }
    }
}