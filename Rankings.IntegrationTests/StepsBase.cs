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
using Rankings.Core.Services.ToBeObsolete;
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
            _gamesService = new GamesService(repository);
            var options = new MemoryCacheOptions();
            var optionsAccessor = Options.Create(options);
            _memoryCache = new MemoryCache(optionsAccessor);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(foo => foo.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), "ProfileEditPolicy"))
                .ReturnsAsync(AuthorizationResult.Success());

            ProfilesController = new ProfilesController(httpContextAccessor.Object, _gamesService, authorizationService.Object);
            VenuesController = new VenuesController(_gamesService);
            GameTypesController = new GameTypesController(_gamesService);

            GamesController = new GamesController(_gamesService, authorizationService.Object, _memoryCache, logger);
        }

        protected RankingsController CreateRankingController(EloConfiguration eloConfiguration, int precision)
        {
            var logger1 = _factory.CreateLogger<IStatisticsService>();
            var logger2 = _factory.CreateLogger<EloCalculator>();
            var eloCalculator = new EloCalculator(eloConfiguration, logger2);
            var oldRankingStatisticsService = new StatisticsService(_gamesService, eloConfiguration, logger1, eloCalculator);
            IStatisticsService rankingService = new NewStatisticsService(_gamesService, eloConfiguration, logger1, eloCalculator, oldRankingStatisticsService);

            return new RankingsController(rankingService, _memoryCache);
        }
    }
}