using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.InMemory;
using Rankings.Web.Controllers;
using Rankings.Web.Models;
using Serilog;
using Serilog.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace Rankings.IntegrationTests
{
    public class RankingFeatureContext
    {
        public bool? MarginOfVictory { get; set; }
        public int? Kfactor { get; set; }
        public int? N { get; set; }
        public int? InitialElo { get; set; }

        public RankingFeatureContext()
        {
        }
    }

    [Binding]
    public class RankingSteps : StepsBase
    {
        private readonly RankingFeatureContext _featureContext;

        public RankingSteps(RankingFeatureContext featureContext, ITestOutputHelper output) : base(output)
        {
            StackTrace stackTrace = new StackTrace();
            output.WriteLine("Feature: " + stackTrace.ToString());
            _featureContext = featureContext;
        }

        public static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
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
            var viewResult = VenuesController.Index() as ViewResult;
            var viewModel = viewResult?.Model as IEnumerable<VenueViewModel>;
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

            var context = ResolveRankingFeatureContext();

            if (context.Kfactor.HasValue)
                throw new Exception("Value cannot be known yet");

            context.InitialElo = initialElo;
            context.N = n;
            context.Kfactor = kfactor;
        }

        private RankingFeatureContext ResolveRankingFeatureContext()
        {
            // if (!ScenarioContext.Current.ContainsKey("context"))
            //     ScenarioContext.Current.Add("context", new RankingFeatureContext());

            //return (RankingFeatureContext) _featureContext.StepContext["context"];
            return _featureContext;
        }

        [Given(@"margin of victory active")]
        public void GivenMarginOfVictoryActive()
        {
            ResolveRankingFeatureContext().MarginOfVictory = true;
        }

        [Given(@"margin of victory is not active")]
        public void GivenMarginOfVictoryIsNotActive()
        {
            ResolveRankingFeatureContext().MarginOfVictory = false;
        }

        [When(@"the following (.*) games are played in (.*):")]
        public void WhenTheFollowingGAMETYPEGamesArePlayedInVENUE(string gameType, string venue, Table table)
        {
            var list = table.CreateSet<TestGame>();

            var viewResult = GamesController.Create() as ViewResult;
            var viewModel = viewResult.Model as GameViewModel;

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
            var context = ResolveRankingFeatureContext();
            if (!context.Kfactor.HasValue || !context.N.HasValue || !context.MarginOfVictory.HasValue || !context.InitialElo.HasValue)
                throw new Exception("Calculator config missing");
            
            Output.Information($"Calculate ranking with k={context.Kfactor} n={context.N}");
            var eloConfiguration = new EloConfiguration(context.Kfactor.Value, context.N.Value, context.MarginOfVictory.Value, context.InitialElo.Value);
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
        private readonly ILoggerFactory _factory;
        protected readonly ILogger Output;

        protected StepsBase(ITestOutputHelper output)
        {
            // Pass the ITestOutputHelper object to the TestOutput sink
            Output = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<StepsBase>();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog(Output))
                .BuildServiceProvider();
            _factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = _factory.CreateLogger<GamesController>();

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

            GamesController = new GamesController(_gamesService, authorizationService.Object, _memoryCache, logger);
        }

        protected RankingsController CreateRankingController(EloConfiguration eloConfiguration, int precision)
        {
            var logger1 = _factory.CreateLogger<StatisticsService>();
            var logger2 = _factory.CreateLogger<EloCalculator>();
            IStatisticsService rankingService = new StatisticsService(_gamesService, eloConfiguration, logger1, new EloCalculator(eloConfiguration, logger2));

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
