using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rankings.Core.Commands;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;
using Profile = Rankings.Core.Entities.Profile;

namespace Rankings.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesProjection _gamesProjection;
        private readonly IAuthorizationService _authorizationService;

        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GamesController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        // TODO move to some central place. Now "GameEditPolicy" string is mentioned twice in the code base. DRY!!
        private const string GameEditPolicy = "GameEditPolicy";

        public GamesController(IGamesProjection gamesProjection, IAuthorizationService authorizationService,
            IMemoryCache memoryCache, ILogger<GamesController> logger, IPublishEndpoint publishEndpoint)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet("/games")]
        [HttpGet("/games/{gametype}")]
        public IActionResult Index(string gameType)
        {
            _logger.LogInformation("Get index page games");

            var model = CreateGameSummaryViewModels(gameType);

            return View(model);
        }

        [HttpGet("/games/doubles")]
        public IActionResult IndexDoubles(string gameType)
        {
            var model = CreateDoubleGameSummaryViewModels(gameType);

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

        private List<DoubleGameViewModel> CreateDoubleGameSummaryViewModels(string gameType)
        {
            var games = _gamesProjection
                .List(new DoubleGamesForPeriodSpecification())
                .ToList();

            var model = games.Select(game => new DoubleGameViewModel
            {
                Id = game.Id,
                GameType = game.GameType.DisplayName,
                Venue = game.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayerFirstTeam = game.Score1 > game.Score2 ? game.Player1Team1.DisplayName : game.Player1Team2.DisplayName,
                NameSecondPlayerFirstTeam = game.Score1 > game.Score2 ? game.Player2Team1.DisplayName : game.Player2Team2.DisplayName,
                NameFirstPlayerSecondTeam = game.Score1 < game.Score2 ? game.Player1Team1.DisplayName : game.Player1Team2.DisplayName,
                NameSecondPlayerSecondTeam = game.Score1 < game.Score2 ? game.Player2Team1.DisplayName : game.Player2Team2.DisplayName,
                RegistrationDate = RegistrationDate(game.RegistrationDate),
                ScoreFirstTeam = game.Score1 > game.Score2 ? game.Score1 : game.Score2,
                ScoreSecondTeam = game.Score1 > game.Score2 ? game.Score2 : game.Score1,
            }).ToList();

            return model;
        }

        private List<GameViewModel> CreateGameSummaryViewModels(string gameType)
        {
            gameType ??= "tafeltennis";
            var daysBack = gameType == "tafeltennis" ? -365 : -365;

            var games = _gamesProjection
                .List(new GamesForPeriodSpecification(gameType, DateTime.Now.AddDays(daysBack), DateTime.MaxValue))
                .OrderByDescending(game => game.RegistrationDate)
                .Take(300)
                .ToList();

            var model = games.Select(type => new GameViewModel
            {
                //Id = type.Id,
                Identifier = type.Identifier.ToString(),
                GameType = type.GameType.DisplayName,
                Venue = type.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayer = type.Score1 > type.Score2 ? type.Player1.DisplayName : type.Player2.DisplayName,
                NameSecondPlayer = type.Score1 > type.Score2 ? type.Player2.DisplayName : type.Player1.DisplayName,
                // TODO fix issue with dates. Add timezone
                RegistrationDate = RegistrationDate(type.RegistrationDate),
                ScoreFirstPlayer = type.Score1 > type.Score2 ? type.Score1 : type.Score2,
                ScoreSecondPlayer = type.Score1 > type.Score2 ? type.Score2 : type.Score1,
            }).ToList();

            var doubles = _gamesProjection
                .List(new DoubleGamesForPeriodSpecification())
                .ToList();

            var model2 = doubles.Select(game => new GameViewModel
            {
                //Id = game.Id,
                Identifier = "",
                GameType = game.GameType.DisplayName,
                Venue = game.Venue?.DisplayName ?? "Unknown",
                NameFirstPlayer = game.Score1 > game.Score2 ? game.Player1Team1.DisplayName + "/" + game.Player2Team1.DisplayName : game.Player1Team2.DisplayName + "/" + game.Player2Team2.DisplayName,
                NameSecondPlayer = game.Score1 < game.Score2 ? game.Player1Team1.DisplayName + "/" + game.Player2Team1.DisplayName : game.Player1Team2.DisplayName + "/" + game.Player2Team2.DisplayName,
                RegistrationDate = RegistrationDate(game.RegistrationDate),
                ScoreFirstPlayer = game.Score1 > game.Score2 ? game.Score1 : game.Score2,
                ScoreSecondPlayer = game.Score1 > game.Score2 ? game.Score2 : game.Score1,
            }).ToList();
            return model.Union(model2).OrderByDescending(viewModel => viewModel.Id).ToList();
        }

        private static string RegistrationDate(DateTimeOffset gameRegistrationDate)
        {
            var registrationDate = gameRegistrationDate;
            return registrationDate.ToString("yyyy/MM/dd H:mm");
        }

        private static string RegistrationDate(DateTime gameRegistrationDate)
        {
            var registrationDate = gameRegistrationDate;
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
                GameTypes = _gamesProjection.List(new AllGameTypes()).Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesProjection.List(new AllVenues()).Select(type => new SelectListItem(type.DisplayName, type.Code))
            });
        }

        private IEnumerable<SelectListItem> OponentPlayers(string nameCurrentUser)
        {
            BaseSpecification<Profile> query = IsAdmin() ? (BaseSpecification<Profile>)new AllProfiles() : new Oponents(nameCurrentUser);

            return _gamesProjection.List(query)
                .Where(profile => profile.IsActive)
                .OrderBy(profile => profile.DisplayName)
                .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));
        }

        private IEnumerable<SelectListItem> FirstPlayers(string nameCurrentUser)
        {
            BaseSpecification<Profile> query = IsAdmin() ? (BaseSpecification<Profile>)new AllProfiles() : new SpecificProfile(nameCurrentUser);

            return _gamesProjection.List(query)
                .OrderBy(profile => profile.DisplayName)
                .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress));
        }

        private IEnumerable<SelectListItem> AllActivePlayers()
        {
            BaseSpecification<Profile> query = new AllProfiles();

            return _gamesProjection.List(query)
                .Where((profile, i) => profile.IsActive)
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
        public async Task<IActionResult> Create(GameViewModel model)
        {
            _logger.LogInformation("Register new game {@Game}", model);

            if (!ModelState.IsValid)
            {
                var currentPlayer = _gamesProjection.Item(new SpecificProfile(User.Identity.Name));
                var oponentPlayers = _gamesProjection.List(new Oponents(User.Identity.Name))
                    .Where(profile => profile.IsActive)
                    .OrderBy(profile => profile.DisplayName)
                    .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress))
                    .ToList();

                model.OpponentPlayers = oponentPlayers;
                model.GameTypes = _gamesProjection.List(new AllGameTypes()).Select(type => new SelectListItem(type.DisplayName, type.Code));
                model.Venues = _gamesProjection.List(new AllVenues()).Select(type => new SelectListItem(type.DisplayName, type.Code));
                model.Players = new[] { new SelectListItem(currentPlayer.DisplayName, currentPlayer.EmailAddress) };

                return View(model);
            }

            var firstPlayer = _gamesProjection.Item(new SpecificProfile(model.NameFirstPlayer));
            var secondPlayer = _gamesProjection.Item(new SpecificProfile(model.NameSecondPlayer));
            var registerSingleGameCommand = new RegisterSingleGameCommand
            {
                FirstPlayer = Guid.Parse(firstPlayer.Identifier),
                SecondPlayer = Guid.Parse(secondPlayer.Identifier),
                ScoreFirstPlayer = model.ScoreFirstPlayer,
                SetScoresFirstPlayer = SerializeSets1(model),
                ScoreSecondPlayer = model.ScoreSecondPlayer,
                SetScoresSecondPlayer = SerializeSets2(model),
                GameType = _gamesProjection.Item(new SpecificGameType(model.GameType)).Code,
                Venue = _gamesProjection.Item(new SpecificVenue(model.Venue)).Code,
                RegistrationDate = DateTimeOffset.UtcNow
            };

            await _publishEndpoint.Publish(registerSingleGameCommand);

            ClearCache();

            Thread.Sleep(500);
            return RedirectToAction("Index");
        }

        private string SerializeSets1(GameViewModel model)
        {
            var sets = new List<string> { model.ScoreFirstPlayerSet1, model.ScoreFirstPlayerSet2, model.ScoreFirstPlayerSet3, model.ScoreFirstPlayerSet4, model.ScoreFirstPlayerSet5 }
                .TakeWhile(s => !string.IsNullOrEmpty(s))
                .Select(int.Parse);

            return string.Join(';', sets);
        }

        private string SerializeSets2(GameViewModel model)
        {

            var sets = new List<string> { model.ScoreSecondPlayerSet1, model.ScoreSecondPlayerSet2, model.ScoreSecondPlayerSet3, model.ScoreSecondPlayerSet4, model.ScoreSecondPlayerSet5 }
                .TakeWhile(s => !string.IsNullOrEmpty(s))
                .Select(int.Parse);

            return string.Join(';', sets);
        }


        [HttpGet("/games/createdouble")]
        public IActionResult CreateDouble()
        {
            return View(new DoubleGameViewModel
            {
                Players = AllActivePlayers(),
                GameTypes = _gamesProjection.List(new AllGameTypes()).Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesProjection.List(new AllVenues()).Select(type => new SelectListItem(type.DisplayName, type.Code)),
            });
        }

        [HttpPost]
        public IActionResult CreateDouble(DoubleGameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Players = AllActivePlayers();
                model.GameTypes = _gamesProjection.List(new AllGameTypes()).Select(type => new SelectListItem(type.DisplayName, type.Code));
                model.Venues = _gamesProjection.List(new AllVenues()).Select(type => new SelectListItem(type.DisplayName, type.Code));

                return View(model);
            }

            var game = new DoubleGame
            {
                GameType = _gamesProjection.Item(new SpecificGameType(model.GameType)),
                Venue = _gamesProjection.Item(new SpecificVenue(model.Venue)),
                Player1Team1 = _gamesProjection.Item(new SpecificProfile(model.NameFirstPlayerFirstTeam)),
                Player2Team1 = _gamesProjection.Item(new SpecificProfile(model.NameSecondPlayerFirstTeam)),
                Player1Team2 = _gamesProjection.Item(new SpecificProfile(model.NameFirstPlayerSecondTeam)),
                Player2Team2 = _gamesProjection.Item(new SpecificProfile(model.NameSecondPlayerSecondTeam)),
                Score1 = model.ScoreFirstTeam,
                Score2 = model.ScoreSecondTeam
            };

            _gamesProjection.RegisterDoubleGame(game);

            return RedirectToAction("IndexDoubles");
        }

        private void ClearCache()
        {
            _memoryCache.Remove("ranking-tafeltennis-2020");
            _memoryCache.Remove("ranking-tafeltennis-eternal");
            _memoryCache.Remove("homepage");
        }

        public IActionResult ResetCache()
        {
            ClearCache();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, GameEditPolicy);
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            var game = _gamesProjection.Item(new SpecificGame(id));

            // TODO use auto mapper

            var sets1 = (game.SetScores1 ?? "").Split(';').ToList();
            var sets2 = (game.SetScores2 ?? "").Split(';').ToList();

            var viewModel = new GameViewModel
            {
                Players = _gamesProjection.List(new AllProfiles())
                    .OrderBy(profile => profile.DisplayName)
                    .Select(profile => new SelectListItem(profile.DisplayName, profile.EmailAddress)),
                GameTypes = _gamesProjection.List(new AllGameTypes())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code)),
                Venues = _gamesProjection.List(new AllVenues())
                    .Select(type => new SelectListItem(type.DisplayName, type.Code)),

                Id = game.Id,
                Identifier = game.Identifier.ToString(),
                RegistrationDate = game.RegistrationDate.ToString(CultureInfo.InvariantCulture),
                NameFirstPlayer = game.Player1.EmailAddress,
                NameSecondPlayer = game.Player2.EmailAddress,
                GameType = game.GameType.Code,
                ScoreFirstPlayer = game.Score1,
                ScoreSecondPlayer = game.Score2,
                ScoreFirstPlayerSet1 = ScoreSet(sets1, 0),
                ScoreSecondPlayerSet1 = ScoreSet(sets2, 0),
                ScoreFirstPlayerSet2 = ScoreSet(sets1, 1),
                ScoreSecondPlayerSet2 = ScoreSet(sets2, 1),
                ScoreFirstPlayerSet3 = ScoreSet(sets1, 2),
                ScoreSecondPlayerSet3 = ScoreSet(sets2, 2),
                ScoreFirstPlayerSet4 = ScoreSet(sets1, 3),
                ScoreSecondPlayerSet4 = ScoreSet(sets2, 3),
                ScoreFirstPlayerSet5 = ScoreSet(sets1, 4),
                ScoreSecondPlayerSet5 = ScoreSet(sets2, 4),
            };

            return View(viewModel);
        }

        private static string ScoreSet(List<string> sets, int index)
        {
            if (sets.Count <= index)
                return "";

            return sets[index];
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GameViewModel model)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, model.Identifier, GameEditPolicy);
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            var game = _gamesProjection.Item(new SpecificGame(model.Identifier));

            if (model.ScoreFirstPlayer != game.Score1 || model.ScoreSecondPlayer != game.Score2)
                _memoryCache.Remove("ranking-" + game.GameType.Code);

            // TODO use auto mapper
            game.Venue = _gamesProjection.Item(new SpecificVenue(model.Venue));
            game.Score1 = model.ScoreFirstPlayer;
            game.Score2 = model.ScoreSecondPlayer;
            game.Player2 = _gamesProjection.Item(new SpecificProfile(model.NameSecondPlayer));

            _gamesProjection.Save(game);
            ClearCache();

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int id)
        {
            _gamesProjection.DeleteGame(id);
            ClearCache();
            return RedirectToAction("AdminIndex");
        }
    }
}