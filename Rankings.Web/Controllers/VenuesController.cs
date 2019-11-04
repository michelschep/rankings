using System;
using System.Linq;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public VenuesController(IGamesService gamesService)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            // TODO inject?
            _mapper = CreateMapper();
        }

        public IActionResult Index()
        {
            var model = _gamesService.List(new AllVenues())
                .Select(venue => _mapper.Map<Venue, VenueViewModel>(venue)).ToList();

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
            var viewModel = _mapper.Map<Venue, VenueViewModel>(venue);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(VenueViewModel viewModel)
        {
            var venue = _gamesService.Item(new SpecificVenue(viewModel.Id));
            _mapper.Map(viewModel, venue);

            _gamesService.Save(venue);

            return RedirectToAction("Index");
        }

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Venue, VenueViewModel>();
                cfg.CreateMap<VenueViewModel, Venue>();
            }).CreateMapper();
        }
    }
}