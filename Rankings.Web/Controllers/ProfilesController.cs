using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Core.Specifications;
using Rankings.Web.Models;
using Profile = Rankings.Core.Entities.Profile;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IStatisticsService _statisticsService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public ProfilesController(IGamesService gamesService, IStatisticsService statisticsService, IAuthorizationService authorizationService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            // TODO inject?
            _mapper = CreateMapper();
        }

        public IActionResult Index()
        {
            var list = _gamesService.List(new AllProfiles())
                .Select(profile => _mapper.Map<Profile, ProfileViewModel>(profile));

            return View(list);
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Create(ProfileViewModel viewModel)
        {
            var profile = _mapper.Map<ProfileViewModel, Profile>(viewModel);
            _gamesService.CreateProfile(profile);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var profile = _gamesService.Item(new SpecificProfile(id));
            var resolveProfileViewModel = _mapper.Map<Profile, ProfileViewModel>(profile);

            var authResult = await _authorizationService.AuthorizeAsync(User, resolveProfileViewModel, "ProfileEditPolicy");
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(resolveProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel profileViewModel)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, profileViewModel, "ProfileEditPolicy");
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _gamesService.UpdateDisplayName(profileViewModel.EmailAddress, profileViewModel.DisplayName);
                return View("Details", profileViewModel);
            }

            return View(profileViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (IsAdmin())
                _gamesService.DeactivateProfile(id);

            return RedirectToAction("Index");
        }

        // TODO duplicated code. Improve
        private bool IsAdmin()
        {
            var isAdmin = User.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == Roles.Admin);
            return isAdmin;
        }

        public IActionResult Details(int id)
        {
            var profile = _gamesService.Item(new SpecificProfile(id));
            var viewModel = _mapper.Map<Profile, ProfileViewModel>(profile);
            var streaks = _statisticsService
                .WinningStreaksPlayer(profile, DateTime.MinValue, DateTime.MaxValue)
                .OrderByDescending(streak=>streak.NumberOfGames);

            var streakViewModels = streaks.Select(streak => new StreakViewModel
            {
                NumberOfGames = streak.NumberOfGames.ToString(),
                EndDate = streak.EndDate.ToString("yyyy/MM/dd H:mm"),
                Player = streak.Player.DisplayName,
                StartDate = streak.StartDate.ToString("yyyy/MM/dd H:mm"),
                NumberOfDays = Math.Round((streak.EndDate - streak.StartDate).TotalDays,0),
                AverageElo = streak.AverageElo.Round().ToString(),
                Volume = (streak.AverageElo * streak.NumberOfGames).Round().ToString()
            });

            viewModel.Streaks = streakViewModels;
            ViewBag.Profile = profile.EmailAddress;

            return View(viewModel);
        }

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Profile, ProfileViewModel>();
                cfg.CreateMap<ProfileViewModel, Profile>();
            }).CreateMapper();
        }
    }
}