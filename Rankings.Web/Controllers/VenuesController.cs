using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class VenuesController : Controller
    {
        private readonly IRankingService _rankingService;

        public VenuesController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        public IActionResult Index()
        {
            var gameTypes = _rankingService.GetVenues();
            var model = gameTypes.Select(type => new VenueViewModel
            {
                Id = type.Id,
                Code = type.Code,
                DisplayName = type.DisplayName
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(VenueViewModel model)
        {
            _rankingService.CreateVenue(new Venue
            {
                Code = model.Code,
                DisplayName = model.DisplayName
            });

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var venue = _rankingService.GetVenues().Single(v => v.Id == id);
            var venueViewModel = new EditVenueViewModel()
            {
                Id = venue.Id,
                Code = venue.Code,
                DisplayName = venue.DisplayName
            };
            return View(venueViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditVenueViewModel viewModel)
        {
            var venue = _rankingService.GetVenues().Single(v => v.Id == viewModel.Id);

            venue.DisplayName = viewModel.DisplayName;
            venue.Code = viewModel.Code;

            _rankingService.Save(venue);

            return RedirectToAction("Index");
        }
    }
}