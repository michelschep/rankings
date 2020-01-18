using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;
using Profile = Rankings.Core.Entities.Profile;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public ProfilesController(IGamesService gamesService, IAuthorizationService authorizationService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
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

        public IActionResult Details(int id)
        {
            var profile = _gamesService.Item(new SpecificProfile(id));
            var viewModel = _mapper.Map<Profile, ProfileViewModel>(profile);

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