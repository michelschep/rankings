using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Core.Specifications;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Rankings.ConsoleApp
{
    public static class GameExtenstions
    {
        public static bool WithPlayer(this EloGame eloGame, string player)
        {
            return eloGame.Game.Player1.EmailAddress.ToLower() == player.ToLower() ||
                   eloGame.Game.Player2.EmailAddress.ToLower() == player.ToLower();
        }

        public static bool WithWinner(this EloGame eloGame, string player)
        {
            return
                eloGame.Game.Player1.EmailAddress.ToLower() == player.ToLower() &&
                eloGame.Game.Score1 > eloGame.Game.Score2
                || eloGame.Game.Player2.EmailAddress.ToLower() == player.ToLower() &&
                eloGame.Game.Score2 > eloGame.Game.Score1;
        }

        private class Program
        {
            private static void Main()
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Ranking.Infrastructure", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile("log-{Date}.txt")
                    .CreateLogger();

                var services = new ServiceCollection();
                services.AddLogging(configure => configure.AddSerilog());
                var provider = services.BuildServiceProvider();

                var gamesService = CreateGamesService(provider);
                var games = gamesService.List(new GamesForPeriodSpecification("tafeltennis", DateTime.MinValue, new DateTime(2019, 12, 31)));

                Console.WriteLine("Ready");
                Console.ReadLine();
            }

            private static StatisticsService CreateStatisticsService()
            {
                var services = new ServiceCollection();
                services.AddLogging(configure => configure.AddSerilog());
                var provider = services.BuildServiceProvider();

                var rankingService = CreateGamesService(provider);

                var eloConfiguration = new EloConfiguration(50, 400, true, 1200, 5);

                return new StatisticsService(rankingService, eloConfiguration,
                    provider.GetService<ILogger<StatisticsService>>(), new EloCalculatorFactory());
            }

            private static GamesService CreateGamesService(ServiceProvider provider)
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                    .Build();

                var database = config["Database"];

                Console.WriteLine(database);

                var connectionFactory = new SqLiteDatabaseConnectionFactory();
                var sqLiteRankingContextFactory =
                    new SqLiteRankingContextFactory(connectionFactory, provider.GetService<ILoggerFactory>());
                var repositoryFactory = new RepositoryFactory(sqLiteRankingContextFactory);
                var repository = repositoryFactory.Create(database);

                return new GamesService(repository);
            }
        }

        public class GameTypes
        {
            public static string Tafeltennis = "tafeltennis";
        }
    }
}