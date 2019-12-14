using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IAuthorizationService _authorizationService;

        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GamesController> _logger;

        // TODO move to some central place. Now "GameEditPolicy" string is mentioned twice in the code base. DRY!!
        private const string GameEditPolicy = "GameEditPolicy";

        public GamesController(IGamesService gamesService, IAuthorizationService authorizationService,
            IMemoryCache memoryCache, ILogger<GamesController> logger)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("/games")]
        [HttpGet("/games/{gametype}")]
        public IActionResult Index(string gameType)
        {
            _logger.LogInformation("Get index page games");

            var model = CreateGameSummaryViewModels(gameType);

            return View(model);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("/adminindex")]
        [HttpGet("/adminindex/{gametype}")]
        public IActionResult AdminIndex(string gameType)
        {
            var model = CreateGameSummaryViewModels(gameType);

            return View(model);
        }

        private List<GameViewModel> CreateGameSummaryViewModels(string gameType)
        {
            gameType ??= "tafeltennis";
            var daysBack = gameType == "tafeltennis" ? -7 : -365;

            var games = _gamesService
                .List(new GamesForPeriodSpecification(gameType, DateTime.Now.AddDays(daysBack), DateTime.MaxValue))
                .OrderByDescending(game => game.RegistrationDate)
                .ToList();

            var model = games.Select(type => new GameViewModel
            {
                Id = type.Id,
                GameType = type.GameType.DisplayName,
                Venue = type.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayer = type.Score1 > type.Score2 ? type.Player1.DisplayName : type.Player2.DisplayName,
                NameSecondPlayer = type.Score1 > type.Score2 ? type.Player2.DisplayName : type.Player1.DisplayName,
                // TODO fix issue with dates. Add timezone
                RegistrationDate = RegistrationDate(type),
                ScoreFirstPlayer = type.Score1 > type.Score2 ? type.Score1 : type.Score2,
                ScoreSecondPlayer = type.Score1 > type.Score2 ? type.Score2 : type.Score1,
            }).ToList();
            return model;
        }

        private static string RegistrationDate(Game game)
        {
            var registrationDate = game.RegistrationDate;
            var correction = registrationDate > new DateTime(2019, 10, 31) ? 1 : 2;
            return registrationDate.AddHours(correction).ToString("yyyy/MM/dd H:mm");
        }

        [HttpGet("/games/create")]
        public IActionResult Create()
        {
            var nameCurrentUser = ResolveCurrentUserName();

            // TODO auto mapper
            return View(new GameViewModel
            {
                NameFirstPlayer = nameCurrentUser,
                Players = FirstPlayers(nameCurrentUser),
                OpponentPlayers = OponentPlayers(nameCurrentUser),
                GameTypes = _gamesService.List(new AllGameTypes()).Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesService.List(new AllVenues()).Select(type => new SelectListItem(type.DisplayName, type.Code))
            });
        }

        private IEnumerable<SelectListItem> OponentPlayers(string nameCurrentUser)
        {
            BaseSpecification<Profile> query = IsAdmin() ? (BaseSpecification<Profile>)new AllProfiles() : new Oponents(nameCurrentUser);

            return _gamesService.List(query)
                .OrderBy(profile => profile.DisplayName)
                .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));
        }

        private IEnumerable<SelectListItem> FirstPlayers(string nameCurrentUser)
        {
            BaseSpecification<Profile> query = IsAdmin() ? (BaseSpecification<Profile>)new AllProfiles() : new SpecificProfile(nameCurrentUser);

            return _gamesService.List(query)
                .OrderBy(profile => profile.DisplayName)
                .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));
        }

        private bool IsAdmin()
        {
            var isAdmin = User.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == Roles.Admin);
            return isAdmin;
        }

        private string ResolveCurrentUserName()
        {
            try
            {
                return User.Identity.Name;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot resolve current user name", ex);
            }
        }

        [HttpPost]
        public IActionResult Create(GameViewModel model)
        {
            _logger.LogInformation("Create Game");

            if (!ModelState.IsValid)
            {
                var currentPlayer = _gamesService.Item(new SpecificProfile(User.Identity.Name));
                var oponentPlayers = _gamesService.List(new Oponents(User.Identity.Name))
                    .OrderBy(profile => profile.DisplayName)
                    .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));

                model.OpponentPlayers = oponentPlayers;
                model.GameTypes = _gamesService.List(new AllGameTypes())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code));
                model.Venues = _gamesService.List(new AllVenues())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code));
                model.Players = new[] {new SelectListItem(currentPlayer.DisplayName, currentPlayer.EmailAddress)};
                return View(model);
            }

            // TODO use auto mapper
            var game = new Game
            {
                GameType = _gamesService.Item(new SpecificGameType(model.GameType)),
                Venue = _gamesService.Item(new SpecificVenue(model.Venue)),
                Player1 = _gamesService.Item(new SpecificProfile(model.NameFirstPlayer)),
                Player2 = _gamesService.Item(new SpecificProfile(model.NameSecondPlayer)),
                Score1 = model.ScoreFirstPlayer,
                Score2 = model.ScoreSecondPlayer,
            };

            _gamesService.RegisterGame(game);

            _memoryCache.Remove("ranking-" + game.GameType.Code);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, GameEditPolicy);
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index", "Rankings");
            }

            var game = _gamesService.Item(new SpecificGame(id));
            // TODO use auto mapper
            var viewModel = new GameViewModel
            {
                Players = _gamesService.List(new AllProfiles())
                    .OrderBy(profile => profile.DisplayName)
                    .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress)),
                GameTypes = _gamesService.List(new AllGameTypes())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesService.List(new AllVenues())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code)),

                Id = game.Id,
                RegistrationDate = game.RegistrationDate.ToString(CultureInfo.InvariantCulture),
                NameFirstPlayer = game.Player1.EmailAddress,
                NameSecondPlayer = game.Player2.EmailAddress,
                GameType = game.GameType.Code,
                ScoreFirstPlayer = game.Score1,
                ScoreSecondPlayer = game.Score2,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GameViewModel model)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, model.Id, GameEditPolicy);
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index", "Rankings");
            }

            var game = _gamesService.Item(new SpecificGame(model.Id));

            if (model.ScoreFirstPlayer != game.Score1 || model.ScoreSecondPlayer != game.Score2)
                _memoryCache.Remove("ranking-" + game.GameType.Code);

            // TODO use auto mapper
            game.Venue = _gamesService.Item(new SpecificVenue(model.Venue));
            game.Score1 = model.ScoreFirstPlayer;
            game.Score2 = model.ScoreSecondPlayer;
            _gamesService.Save(game);

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int id)
        {
            _gamesService.DeleteGame(id);
            return View("Index");
        }
    }
}