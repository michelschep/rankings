using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;
using Serilog;

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
                (eloGame.Game.Player1.EmailAddress.ToLower() == player.ToLower() &&
                 eloGame.Game.Score1 > eloGame.Game.Score2)
                || (eloGame.Game.Player2.EmailAddress.ToLower() == player.ToLower() &&
                    eloGame.Game.Score2 > eloGame.Game.Score1);
        }

        class Program
        {
            static void Main()
            {
                var statsService = CreateStatisticsService();

                // show current ranking
                var ranking = statsService.TheNewRanking(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);
                foreach (var item in ranking)
                {
                    Console.WriteLine(
                        $"{item.Value.Ranking,2}. {item.Key.DisplayName,-20} {item.Value.NumberOfGames,3} {item.Value.EloScore.Round(),4}");
                }

                // Show games one player
                var games = statsService.EloGames(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);
                foreach (var eloGame in games.OrderBy(game => Math.Abs(game.Player1Delta)))
                {
                    Console.WriteLine(
                        $"{eloGame.Game.Player1.DisplayName}-{eloGame.Game.Player2.DisplayName} ==> {eloGame.Player1Delta.Round()}:{eloGame.Player2Delta.Round()}");
                }

                Console.WriteLine("=====");

                var jflamelingVitasNl = "jflameling@vitas.nl";
                var gpostmaVitasNl = "mschep@vitas.nl";
                var games2 = games
                    .Where((game, i) => game.WithPlayer(jflamelingVitasNl) && game.WithPlayer(gpostmaVitasNl))
                    .ToList();
                foreach (var eloGame in games2.OrderBy(game => Math.Abs(game.Player1Delta)))
                {
                    Console.WriteLine(
                        $"{eloGame.Game.Player1.DisplayName}-{eloGame.Game.Player2.DisplayName} ==> {eloGame.Player1Delta.Round()}:{eloGame.Player2Delta.Round()}");
                }

                var winst = games2.Where(g => g.WithWinner(jflamelingVitasNl)).Sum(g => Math.Abs(g.Player1Delta));
                var verlies = games2.Where(g => g.WithWinner(gpostmaVitasNl)).Sum(g => Math.Abs(g.Player1Delta));

                Console.WriteLine($"{winst}-{verlies}");

                // show previous ranking

                // show current goat score
                //var goatScores = statsService.GoatScores(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);

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

                var rankingService = CreateGamesService(provider);

                var eloConfiguration = new EloConfiguration(50, 400, true, 1200);
                var oldStatsService = new OldStatisticsService(rankingService, eloConfiguration,
                    provider.GetService<ILogger<OldStatisticsService>>(),
                    new EloCalculator(eloConfiguration, provider.GetService<ILogger<EloCalculator>>()));

                return new NewStatisticsService(rankingService, eloConfiguration,
                    provider.GetService<ILogger<NewStatisticsService>>(),
                    new EloCalculator(eloConfiguration, provider.GetService<ILogger<EloCalculator>>()),
                    oldStatsService);
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
                var sqLiteRankingContextFactory = new SqLiteRankingContextFactory(connectionFactory, provider.GetService<ILoggerFactory>());
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
}