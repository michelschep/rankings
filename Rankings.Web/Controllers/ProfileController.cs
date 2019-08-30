using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
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

    public interface IRankingService
    {
        IEnumerable<Profile> Profiles();
        void ActivateProfile(string email, string displayName);
        Profile ProfileFor(string email);
        void UpdateDisplayName(string emailAddress, string displayName);
    }

    class TestRankingService : IRankingService
    {
        private readonly List<Profile> _repository;

        public TestRankingService()
        {
            _repository = new List<Profile>();
        }

        public IEnumerable<Profile> Profiles()
        {
            return _repository;
        }

        public void ActivateProfile(string email, string displayName)
        {
            if (_repository.Any(profile => profile.EmailAddress == email))
                return;

            _repository.Add(new Profile(email, displayName));
        }

        public Profile ProfileFor(string email)
        {
            return _repository.Single(profile => profile.EmailAddress == email);
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = _repository.Single(p => p.EmailAddress == emailAddress);

            _repository.Remove(profile);
            _repository.Add(new Profile(emailAddress, displayName));
        }
    }

    public class Profile
    {
        public string EmailAddress { get; }

        public string DisplayName { get; }

        public Profile(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }
    }
}