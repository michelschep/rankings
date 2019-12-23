using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var statsService = CreateStatisticsService();

            // show current ranking

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

        private static OldStatisticsService CreateStatisticsService()
        {
            var rankingService = CreateGamesService();

            var eloConfiguration = new EloConfiguration(50, 400, true, 1200);
            var statsService = new OldStatisticsService(rankingService, eloConfiguration, null, new EloCalculator(eloConfiguration, null));
            return statsService;
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
}
