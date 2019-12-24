using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;
using Serilog;

namespace Rankings.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(@".\ranKINGS.json", shared: true)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddSerilog());

            var statsService = CreateStatisticsService();

            // show current ranking
            var ranking = statsService.EloStats(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);
            foreach (var item in ranking)
            {
                Console.WriteLine($"{item.Value.Ranking, 2}. {item.Key.DisplayName, -20} {item.Value.EloScore.Round(), 4}");
            }

            // show previous ranking

            // show current goat score

            // Show current win%

            // Show current streak player

            // Show best streak player

            // Show average winning streak player

            // Show average winning streak all players

            // Show ranking best streaks

            // Show all current running streaks

            // Show games X-Y

            // Show summary games X-Y

            // Show summaries games

            // Show social ranking


            Console.ReadLine();
        }

        private static NewStatisticsService CreateStatisticsService()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(@".\ranKINGS.json", shared: true)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddSerilog());
            var provider = services.BuildServiceProvider();

            var rankingService = CreateGamesService();

            var eloConfiguration = new EloConfiguration(50, 400, true, 1200);
            var oldStatsService = new OldStatisticsService(rankingService, eloConfiguration, provider.GetService<ILogger<OldStatisticsService>>(),
                new EloCalculator(eloConfiguration, provider.GetService<ILogger<EloCalculator>>()));

            return new NewStatisticsService(rankingService, eloConfiguration,
                provider.GetService<ILogger<NewStatisticsService>>(), new EloCalculator(eloConfiguration, provider.GetService<ILogger<EloCalculator>>()),
                oldStatsService);
        }

        private static GamesService CreateGamesService()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .Build();

            var database = config["Database"];

            Console.WriteLine(database);

            var connectionFactory = new SqLiteDatabaseConnectionFactory();
            var sqLiteRankingContextFactory = new SqLiteRankingContextFactory(connectionFactory);
            var repositoryFactory = new RepositoryFactory(sqLiteRankingContextFactory);
            var repository = repositoryFactory.Create(database);
            var gamesService = new GamesService(repository);
            return gamesService;
        }
    }

    public class GameTypes
    {
        public static string Tafeltennis = "tafeltennis";
    }
}