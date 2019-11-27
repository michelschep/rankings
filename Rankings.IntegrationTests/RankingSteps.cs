using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Rankings.Web.Controllers;
using Rankings.Web.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Rankings.IntegrationTests
{
    [Binding]
    public class RankingSteps
    {
        private GamesService _gamesService;

        public RankingSteps()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create(Guid.NewGuid().ToString());
            _gamesService = new GamesService(repository);
        }

        [Given(@"a player named (.*)")]
        public void GivenAPlayerNamedNAME(string name)
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(foo => foo.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<Object>(), "ProfileEditPolicy"))
                .ReturnsAsync(AuthorizationResult.Success());

            var profilesController = new ProfilesController(httpContextAccessor.Object, _gamesService, authorizationService.Object);
            var model = new ProfileViewModel()
            {
                DisplayName = name
            };
            profilesController.Create(model);
        }
        
        [Given(@"a venue named (.*)")]
        public void GivenAVenueNamedNAME(string name)
        {
            var venuesController = new VenuesController(_gamesService);
            var model = new VenueViewModel
            {
                Code = name,
                DisplayName = name
            };
            venuesController.Create(model);
        }

        [Given(@"a game type named (.*)")]
        public void GivenAGameTypeNamedNAME(string gameType)
        {
            var gameTypesController = new GameTypesController(_gamesService);
            var model = new GameTypeViewModel()
            {
                Code = gameType,
                DisplayName = gameType
            };
            gameTypesController.Create(model);
        }


        [Given(@"no games played")]
        public void GivenNoGamesPlayed()
        {
        }
        
        [When(@"nothing happens")]
        public void WhenNothingHappens()
        {
        }

        [Given(@"elo system with k-factor (.*) and n is (.*)")]
        public void GivenEloSystemWithK_FactorAndNIs(int kfactor, int n)
        {
        }

        [When(@"the following (.*) games are played in (.*):")]
        public void WhenTheFollowingGAMETYPEGamesArePlayedInVENUE(string gameType, string venue, Table table)
        {
        }

        [Then(@"we have the following ranking:")]
        public void ThenWeHaveTheFollowingRanking(Table table)
        {
        }
          //  var list = table.CreateSet<TestGame>();
    }

    public class TestGame
    {
        public DateTime RegistrationDate { get; set; }
        public string FirstPlayer { get; set; }
        public string SeondPlayer { get; set; }
        public string SecondPlayer { get; set; }
    }
}
