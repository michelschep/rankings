using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGamesService _gamesService;
        private readonly IAuthorizationService _authorizationService;

        public ProfilesController(IHttpContextAccessor httpContextAccessor, IGamesService gamesService, IAuthorizationService authorizationService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        public IActionResult Index()
        {
            // TODO get rid of this silly thing
            ActivateCurrentUser();

            var list = _gamesService.List(new AllProfiles())
                .Select(profile => new ProfileViewModel
                {
                    Id = profile.Id,
                    EmailAddress = profile.EmailAddress,
                    DisplayName = profile.DisplayName
                });

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
            _gamesService.CreateProfile(new Profile
            {
                EmailAddress = viewModel.EmailAddress,
                DisplayName = viewModel.DisplayName
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            // TODO get rid of this
            ActivateCurrentUser();

            // TODO use mapper
            var profile = _gamesService.Item(new SpecificProfile(id));
            var resolveProfileViewModel = new ProfileViewModel
            {
                Id = profile.Id,
                EmailAddress = profile.EmailAddress,
                DisplayName = profile.DisplayName
            };

            var authResult = await _authorizationService.AuthorizeAsync(User, resolveProfileViewModel, "ProfileEditPolicy");
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index", "Rankings");
            }

            return View(resolveProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel profileViewModel)
        {
            //var profile = _gamesService.Item(new SpecificProfile(profileViewModel.Id));
            var authResult = await _authorizationService.AuthorizeAsync(User, profileViewModel, "ProfileEditPolicy");
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index", "Rankings");
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
            ActivateCurrentUser();

            return View(ResolveProfileViewModel(id));
        }

        private void ActivateCurrentUser()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Surname).Value;

            _gamesService.ActivateProfile(email, name);
        }

        private ProfileViewModel ResolveProfileViewModel(int id)
        {
            //var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var profile = _gamesService.Item(new SpecificProfile(id));

            return new ProfileViewModel
            {
                Id = profile.Id,
                EmailAddress = profile.EmailAddress,
                DisplayName = profile.DisplayName
            };
        }
    }
}