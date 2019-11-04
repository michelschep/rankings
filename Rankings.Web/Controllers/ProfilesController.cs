using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGamesService _gamesService;

        public ProfilesController(IHttpContextAccessor httpContextAccessor, IGamesService gamesService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        public IActionResult Index()
        {
            ActivateCurrentUser();

            var list = _gamesService.Profiles().Select(profile => new ProfileViewModel(profile.EmailAddress, profile.DisplayName));

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateProfileViewModel viewModel)
        {
            _gamesService.CreateProfile(new Profile
            {
                EmailAddress = viewModel.EmailAddress,
                DisplayName = viewModel.DisplayName
            });

            return RedirectToAction("Index");
        }

        public IActionResult Edit()
        {
            ActivateCurrentUser();

            return View(ResolveProfileViewModel());
        }

        [HttpPost]
        public IActionResult Edit(ProfileViewModel profileViewModel)
        {
            _gamesService.UpdateDisplayName(profileViewModel.EmailAddress, profileViewModel.DisplayName);

            return View("Details", profileViewModel);
        }

        public IActionResult Details()
        {
            ActivateCurrentUser();

            return View(ResolveProfileViewModel());
        }

        private void ActivateCurrentUser()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Surname).Value;

            _gamesService.ActivateProfile(email, name);
        }

        private ProfileViewModel ResolveProfileViewModel()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var profile = _gamesService.ProfileFor(email);
            var profileViewModel = new ProfileViewModel(profile.EmailAddress, profile.DisplayName);
            return profileViewModel;
        }
    }
}