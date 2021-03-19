using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    public class EventsController: Controller
    {
        // GET
        public IActionResult Index()
        {
            EventViewModel model = new EventViewModel()
            {
                Index = "1",
                Id = Guid.NewGuid().ToString(),
                CreationDate = DateTime.UtcNow.ToString() 
            };
            
            var list = new List<EventViewModel>() {model};
            return View(list);
        }
    }
}