using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Rankings.Web.Controllers;
using Rankings.Web.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Rankings.IntegrationTests
{
    [Binding]
    public class RankingSteps: StepsBase
    {
        private bool _marginOfVictory = false;
        private int _kfactor = 50;
        private int _n = 400;
        private int _initialElo = 1200;

        [Given(@"no venues registrated")]
        public void GivenNoVenuesRegistrated()
        {
        }
        
        [Given(@"venue (.*) exists")]
        public void GivenVenueAlmereExists(string nameVenue)
        {
            VenuesController.Create(new VenueViewModel
            {
                DisplayName = nameVenue,
                Code = nameVenue
            });
        }

        [When(@"venue (.*) is registrated")]
        public void WhenVenueGroningenIsRegistrated(string nameVenue)
        {
            VenuesController.Create(new VenueViewModel
            {
                DisplayName = nameVenue,
                Code = nameVenue
            });
        }
        
        [Then(@"we have the following venues:")]
        public void ThenWeHaveTheFollowingVenues(Table table)
        {
            var viewResult = VenuesController.Index() as ViewResult;
            var viewModel = viewResult.Model as IEnumerable<VenueViewModel>;
            var venues = viewModel.ToList();

            var expectedVenues = table.CreateSet<VenueViewModel>().ToList();
            // TODO assert expected equals actual
            venues.Should().BeEquivalentTo(expectedVenues, options => options
                .WithStrictOrdering()
                .Including(model => model.DisplayName)
            );
        }

        [When(@"incorrect venue is registrated")]
        public void WhenIncorrectVenueIsRegistrated()
        {
            VenuesController.ModelState.AddModelError("key", "venue view model is not valid");
            VenuesController.Create(new VenueViewModel());
        }

        [Given(@"a player named (.*)")]
        public void GivenAPlayerNamedNAME(string name)
        {
            var model = new ProfileViewModel
            {
                EmailAddress = EmailAddressFor(name),
                DisplayName = name
            };

            ProfilesController.Create(model);
        }

        private static string EmailAddressFor(string name)
        {
            return $"{name}@domain.nl";
        }

        [Given(@"a venue named (.*)")]
        public void GivenAVenueNamedNAME(string name)
        {
            var model = new VenueViewModel
            {
                Code = name,
                DisplayName = name
            };
            VenuesController.Create(model);
        }

        [Given(@"a game type named (.*)")]
        public void GivenAGameTypeNamedNAME(string gameType)
        {
            var model = new GameTypeViewModel
            {
                Code = gameType,
                DisplayName = gameType
            };
            GameTypesController.Create(model);
        }

        [Given(@"the current user is (.*) with role (.*)")]
        public void GivenTheCurrentUserIsMichelWithRolePlayer(string userName, string role)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, EmailAddressFor(userName)),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            GamesController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Given(@"no games played")]
        public void GivenNoGamesPlayed()
        {
        }

        [When(@"nothing happens")]
        public void WhenNothingHappens()
        {
        }

        [When(@"I view the (.*) ranking")]
        public void WhenIViewTheTafeltennisRanking(string gameType)
        {
        }


        [Given(@"elo system with k-factor (.*) and n is (.*) and initial elo is (.*)")]
        public void GivenEloSystemWithK_FactorAndNIsAndInitialEloIs(int kfactor, int n, int initialElo)
        {
            _initialElo = initialElo;
            _n = n;
            _kfactor = kfactor;
        }

        [Given(@"margin of victory active")]
        public void GivenMarginOfVictoryActive()
        {
            _marginOfVictory = true;
        }

        [When(@"the following (.*) games are played in (.*):")]
        public void WhenTheFollowingGAMETYPEGamesArePlayedInVENUE(string gameType, string venue, Table table)
        {
            var list = table.CreateSet<TestGame>();

            var viewResult = GamesController.Create() as ViewResult;
            var viewModel = viewResult.Model as GameViewModel;

            foreach (var game in list)
            {
                var firstPlayer = viewModel.Players.First(item => item.Text == game.FirstPlayer);
                var secondPlayer = viewModel.OpponentPlayers.First(item => item.Text == game.SecondPlayer);

                var model = new GameViewModel
                {
                    RegistrationDate = game.RegistrationDate.ToString(CultureInfo.InvariantCulture),
                    GameType = gameType,
                    NameFirstPlayer = firstPlayer.Value,
                    NameSecondPlayer = secondPlayer.Value,
                    ScoreFirstPlayer = game.S1,
                    ScoreSecondPlayer = game.S2
                };
                GamesController.Create(model);
            }
        }

        [Then(@"we have the following (.*) ranking with precision (.*):")]
        public void ThenWeHaveTheFollowingRanking(string gameType, int precision, Table table)
        {
            var eloConfiguration = new EloConfiguration(_kfactor, _n, _marginOfVictory, _initialElo);
            var rankingController = CreateRankingController(eloConfiguration, precision);
            var viewResult = rankingController.Index(gameType, DateTime.MaxValue.ToString(CultureInfo.InvariantCulture), precision, 0) as ViewResult;
            var viewModel = viewResult.Model as IEnumerable<RankingViewModel>;
            var actualRanking = viewModel.ToList();

            var expectedRanking = table.CreateSet<RankingViewModel>().ToList();
            
            actualRanking.Should().BeEquivalentTo(expectedRanking, options => options
                    .WithStrictOrdering()
                    .Including(model => model.Ranking)
                    .Including(model => model.NamePlayer)
                    .Including(model => model.Points)
                );
        }
    }

    public class StepsBase
    {
        protected readonly ProfilesController ProfilesController;
        protected readonly VenuesController VenuesController;
        protected readonly GameTypesController GameTypesController;
        protected readonly GamesController GamesController;
        private readonly GamesService _gamesService;
        private readonly IMemoryCache _memoryCache;

        protected StepsBase()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _gamesService = new GamesService(repository);
            var options = new MemoryCacheOptions();
            var optionsAccessor = Options.Create(options);
            _memoryCache = new MemoryCache(optionsAccessor);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(foo => foo.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<Object>(), "ProfileEditPolicy"))
                .ReturnsAsync(AuthorizationResult.Success());

            ProfilesController = new ProfilesController(httpContextAccessor.Object, _gamesService, authorizationService.Object);
            VenuesController = new VenuesController(_gamesService);
            GameTypesController = new GameTypesController(_gamesService);
           
            GamesController = new GamesController(_gamesService, authorizationService.Object, _memoryCache);
        }

        protected RankingsController CreateRankingController(EloConfiguration eloConfiguration, int precision)
        {
            IStatisticsService rankingService = new StatisticsService(_gamesService, eloConfiguration);
            return new RankingsController(rankingService, _memoryCache);
        }
    }

    public class TestGame
    {
        public DateTime RegistrationDate { get; set; }
        public string FirstPlayer { get; set; }
        public string SecondPlayer { get; set; }
        public int S1 { get; set; }
        public int S2 { get; set; }
    }
}
