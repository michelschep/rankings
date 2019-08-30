using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize(Policy = "Player")]
    public class ProfilesController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRankingService _rankingService;

        public ProfilesController(IHttpContextAccessor httpContextAccessor, IRankingService rankingService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            ActivateCurrentUser();

            var list = _rankingService.Profiles().Select(profile => new ProfileViewModel(profile.EmailAddress, profile.DisplayName));

            return View(list);
        }

        public IActionResult Edit()
        {
            ActivateCurrentUser();

            return View(ResolveProfileViewModel());
        }

        [HttpPost]
        public IActionResult Edit(ProfileViewModel profileViewModel)
        {
            _rankingService.UpdateDisplayName(profileViewModel.EmailAddress, profileViewModel.DisplayName);

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

            _rankingService.ActivateProfile(email, name);
        }

        private ProfileViewModel ResolveProfileViewModel()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var profile = _rankingService.ProfileFor(email);
            var profileViewModel = new ProfileViewModel(profile.EmailAddress, profile.DisplayName);
            return profileViewModel;
        }
    }
}