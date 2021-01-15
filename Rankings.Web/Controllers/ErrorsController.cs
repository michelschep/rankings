using Microsoft.AspNetCore.Mvc;

namespace Rankings.Web.Controllers
{
    [Route("errors")]
    public class ErrorsController : Controller
    {
        [Route("500")]
        public IActionResult Error500()
        {
            return View("500");
        }

        [Route("404")]
        public IActionResult Error404()
        {
            return View("404");
        }
    }
}