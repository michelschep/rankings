using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rankings.Core.Services;
using Rankings.Web.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Abstractions;

namespace Rankings.IntegrationTests
{
    [Binding]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class RankingSteps : StepsBase
    {
        private readonly RankingFeatureContext _featureContext;

        public RankingSteps(RankingFeatureContext featureContext, ITestOutputHelper output) : base(output)
        {
            _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        }

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
            var viewResult = (ViewResult) VenuesController.Index();
            var viewModel = (IEnumerable<VenueViewModel>) viewResult.Model;
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
            Output.Information($"Create Player {name}");
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
            Output.Information($"Set k ={kfactor}, n={n}, and initial elo = {initialElo}");

            var context = _featureContext;

            if (context.Kfactor.HasValue)
                throw new Exception("Value cannot be known yet");

            context.InitialElo = initialElo;
            context.N = n;
            context.Kfactor = kfactor;
        }

        [Given(@"margin of victory active")]
        public void GivenMarginOfVictoryActive()
        {
            _featureContext.MarginOfVictory = true;
        }

        [Given(@"only in ranking with a minimum of (.*) games")]
        public void GivenOnlyInRankingWithAMinimumOfGames(int p0)
        {
            _featureContext.MinimumRankingGames = p0;
        }

        [Given(@"margin of victory is not active")]
        public void GivenMarginOfVictoryIsNotActive()
        {
            _featureContext.MarginOfVictory = false;
        }

        [When(@"the following (.*) games are played in (.*):")]
        public void WhenTheFollowingGAMETYPEGamesArePlayedInVENUE(string gameType, string venue, Table table)
        {
            var list = table.CreateSet<TestGame>();

            var viewResult = (ViewResult) GamesController.Create();
            var viewModel = (GameViewModel) viewResult.Model;

            foreach (var game in list)
            {
                var firstPlayer = viewModel.Players.FirstOrDefault(item => item.Text == game.FirstPlayer);
                if (firstPlayer == null)
                    throw new Exception($"Unknown player {game.FirstPlayer}");

                var secondPlayer = viewModel.OpponentPlayers.FirstOrDefault(item => item.Text == game.SecondPlayer);
                if (secondPlayer == null)
                    throw new Exception($"Unknown player {game.SecondPlayer}");

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
            var context = _featureContext;
            if (!context.Kfactor.HasValue || !context.N.HasValue || !context.MarginOfVictory.HasValue || !context.InitialElo.HasValue)
                throw new Exception("Calculator config missing");
            
            Output.Information($"***** Calculate ranking with k={context.Kfactor} n={context.N}");
            var eloConfiguration = new EloConfiguration(context.Kfactor.Value, context.N.Value, context.MarginOfVictory.Value, context.InitialElo.Value, context.MinimumRankingGames)
            {
                JustNumbersForRanking = true
            };
            var rankingController = CreateRankingController(eloConfiguration);
            var viewResult = (ViewResult) rankingController.EternalRanking(precision);
            var viewModel = (IEnumerable<RankingViewModel>) viewResult.Model;
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

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TestGame
    {
        public DateTime RegistrationDate { get; set; }
        public string FirstPlayer { get; set; }
        public string SecondPlayer { get; set; }
        public int S1 { get; set; }
        public int S2 { get; set; }
    }
}
