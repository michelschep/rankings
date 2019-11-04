﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;
using Rankings.Web.Models;

namespace Rankings.Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class VenuesController : Controller
    {
        private readonly IGamesService _gamesService;

        public VenuesController(IGamesService gamesService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        public IActionResult Index()
        {
            var model = _gamesService.List(new AllVenues())
                .Select(type => new VenueViewModel
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
            _gamesService.CreateVenue(new Venue
            {
                Code = model.Code,
                DisplayName = model.DisplayName
            });

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var venue = _gamesService.Item(new SpecificVenue(id));
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
            var venue = _gamesService.Item(new SpecificVenue(viewModel.Id));
            venue.Code = viewModel.Code;
            venue.DisplayName = viewModel.DisplayName;
            _gamesService.Save(venue);

            return RedirectToAction("Index");
        }
    }
}