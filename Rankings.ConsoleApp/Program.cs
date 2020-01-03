using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
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
                eloGame.Game.Player1.EmailAddress.ToLower() == player.ToLower() &&
                eloGame.Game.Score1 > eloGame.Game.Score2
                || eloGame.Game.Player2.EmailAddress.ToLower() == player.ToLower() &&
                eloGame.Game.Score2 > eloGame.Game.Score1;
        }

        private class Program
        {
            private static void Main()
            {
                var statsService = CreateStatisticsService();

                // show current ranking
                var ranking = statsService.Ranking(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);
                foreach (var item in ranking)
                    Console.WriteLine(
                        $"{item.Value.Ranking,2}. {item.Key.DisplayName,-20} {item.Value.NumberOfGames,3} {item.Value.EloScore.Round(),4}");

                // Show games one player
                var games = statsService.EloGames(GameTypes.Tafeltennis, DateTime.MinValue, DateTime.MaxValue);
                foreach (var eloGame in games.OrderBy(game => Math.Abs(game.Player1Delta)))
                    Console.WriteLine(
                        $"{eloGame.Game.Player1.DisplayName}-{eloGame.Game.Player2.DisplayName} ==> {eloGame.Player1Delta.Round()}:{eloGame.Player2Delta.Round()}");

                Console.WriteLine("=====");

                var jflamelingVitasNl = "jflameling@vitas.nl";
                var gpostmaVitasNl = "mschep@vitas.nl";
                var games2 = games
                    .Where((game, i) => game.WithPlayer(jflamelingVitasNl) && game.WithPlayer(gpostmaVitasNl))
                    .ToList();
                foreach (var eloGame in games2.OrderBy(game => Math.Abs(game.Player1Delta)))
                    Console.WriteLine(
                        $"{eloGame.Game.Player1.DisplayName}-{eloGame.Game.Player2.DisplayName} ==> {eloGame.Player1Delta.Round()}:{eloGame.Player2Delta.Round()}");

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
                var result = games.SelectMany((game, i) => new List<EloGame>()
                {
                    game, new EloGame(new Game()
                    {
                        GameType = game.Game.GameType,
                        Player1 = game.Game.Player2,
                        Player2 = game.Game.Player1,
                        Score1 = game.Game.Score2,
                        Score2 = game.Game.Score1,
                        RegistrationDate = game.Game.RegistrationDate,
                        Venue = game.Game.Venue,
                    }, game.EloPlayer2, game.EloPlayer1, game.Player2Delta, game.Player1Delta)
                }).GroupBy(game => game.Game.Player1.EmailAddress);

                var numberOfPlayers = result.Count();
                foreach (var item in result)
                {
                    var eloGames = item.ToList();
                    var op = eloGames.Select(game => game.Game.Player2.EmailAddress).Distinct().Count();

                    var groups = eloGames.GroupBy(game => game.Game.Player2.EmailAddress)
                        .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList().Count);

                    double total = 0;
                    var avg = eloGames.Count() / numberOfPlayers;

                    foreach (var g in groups)
                    {
                        total += Math.Pow(g.Value - avg, 2);
                    }

                    var r = Math.Sqrt(total / numberOfPlayers);

                    Console.WriteLine($"{item.Key, -25}: {eloGames.Count, 5} {op, 3} {(100*op)/numberOfPlayers, 3 } {r}");
                }


                Console.ReadLine();
            }

            private static StatisticsService CreateStatisticsService()
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

                var eloConfiguration = new EloConfiguration(50, 400, true, 1200, 7);

                return new StatisticsService(rankingService, eloConfiguration,
                    provider.GetService<ILogger<StatisticsService>>(),
                    new EloCalculator(eloConfiguration, provider.GetService<ILogger<EloCalculator>>()));
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
                
                return new GamesService(repository);
            }
        }

        public class GameTypes
        {
            public static string Tafeltennis = "tafeltennis";
        }
    }
}