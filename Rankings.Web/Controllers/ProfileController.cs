using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Profile()
        {
            var profileViewModel = new ProfileViewModel("pietje@domain.nl", "Pietje Puk");
            return View(profileViewModel);
        }
    }
}