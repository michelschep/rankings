using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public ProfilesController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var profileViewModel1 = new ProfileViewModel("123", "xyz");
            var profileViewModel2 = new ProfileViewModel("123", "xyz");
            return View(new List<ProfileViewModel>() { profileViewModel1, profileViewModel2});
        }

        public IActionResult Edit()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Surname).Value;

            var profileViewModel = new ProfileViewModel(email, name);
            return View(profileViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ProfileViewModel profileViewModel)
        {
            return View("Details", profileViewModel);
        }

        public IActionResult Details()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Surname).Value;

            var profileViewModel = new ProfileViewModel(email, name);
            return View(profileViewModel);
        }
    }
}