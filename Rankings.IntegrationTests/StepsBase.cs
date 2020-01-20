using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using Serilog;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace Rankings.IntegrationTests
{
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
                .WriteTo.TestOutput(output)
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
            _gamesService = new GamesService(repository, new RankingsClock());
            var options = new MemoryCacheOptions();
            var optionsAccessor = Options.Create(options);
            _memoryCache = new MemoryCache(optionsAccessor);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(foo => foo.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), "ProfileEditPolicy"))
                .ReturnsAsync(AuthorizationResult.Success());

            ProfilesController = new ProfilesController(_gamesService, authorizationService.Object);
            VenuesController = new VenuesController(_gamesService);
            GameTypesController = new GameTypesController(_gamesService);

            GamesController = new GamesController(_gamesService, authorizationService.Object, _memoryCache, logger);
        }

        protected RankingsController CreateRankingController(EloConfiguration eloConfiguration)
        {
            //IEloCalculatorFactory eloCalculatorFactory = new AdHocEloCalculatorFactory(() => new EloCalculatorVersion2019());
            IEloCalculatorFactory eloCalculatorFactory = new EloCalculatorFactory();
            var rankingService = new StatisticsService(_gamesService, eloConfiguration, _factory.CreateLogger<IStatisticsService>(), eloCalculatorFactory);

            return new RankingsController(rankingService, _memoryCache, eloConfiguration);
        }
    }

    public class AdhocClock : IRankingsClock
    {
        private readonly Func<DateTime> _now;

        public AdhocClock(Func<DateTime> now)
        {
            _now = now;
        }

        public DateTime Now()
        {
            return _now();
        }
    }

    public class AdHocEloCalculatorFactory : IEloCalculatorFactory
    {
        private readonly Func<IEloCalculator> _create;

        public AdHocEloCalculatorFactory(Func<IEloCalculator> create)
        {
            _create = create;
        }

        public IEloCalculator Create(int year)
        {
            return _create();
        }
    }
}