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

                var statsService = CreateStatisticsService();
                var eloGames = statsService.EloGames("tafeltennis", DateTime.MinValue, new DateTime(2019, 12, 31));


                // Summary of classic games
                var summary = eloGames.Select(game => game.Game)
                    .SelectMany(game =>
                    {
                        if (game.Player1.Id < game.Player2.Id)
                        {
                            return new[] {new {Player1 = game.Player1.DisplayName, Player2 = game.Player2.DisplayName, game.Score1, game.Score2}}.ToList();
                        }

                        return new[] {new {Player1 = game.Player2.DisplayName, Player2 = game.Player1.DisplayName, Score1 = game.Score2, Score2 = game.Score1}}.ToList();
                    })
                    .GroupBy(arg => new {arg.Player1, arg.Player2})
                    .Select(grouping => new {grouping.Key.Player1, grouping.Key.Player2, Aantal = grouping.Count(), Won = grouping.Count(arg => arg.Score1 > arg.Score2), Sets1 = grouping.Sum(arg => arg.Score1), Sets2 = grouping.Sum(arg => arg.Score2)})
                    //.Where(arg => arg.Aantal >= 10)
                    .OrderByDescending(arg => arg.Player1);

                Console.WriteLine();
                Console.WriteLine();
                foreach (var item in summary)
                {
                    var p1 = Regex.Replace(item.Player1, @"[^\u0000-\u007F]+", string.Empty);
                    var p2 = Regex.Replace(item.Player2, @"[^\u0000-\u007F]+", string.Empty);
                    var winperc = (100m * item.Won / item.Aantal).Round();
                    var setwinperc = (100m * item.Sets1 / (item.Sets1 + item.Sets2)).Round();
                    Log.Logger.Information($"{p1,-20} - {p2,-20} {item.Won,3} - {item.Aantal - item.Won,3} {winperc,3} - {100 - winperc,3} {item.Sets1,3} - {item.Sets2,3} {setwinperc,3} - {100 - setwinperc,3}");
                }

                // Social player
                var summary2 = eloGames.Select(game => game.Game)
                    .SelectMany(game =>
                    {
                        return new[]
                        {
                            new {Player1 = game.Player1.DisplayName, Player2 = game.Player2.DisplayName, game.Score1, game.Score2},
                            new {Player1 = game.Player2.DisplayName, Player2 = game.Player1.DisplayName, Score1 = game.Score2, Score2 = game.Score1}
                        }.ToList();
                    })
                    .GroupBy(arg => arg.Player1)
                    .Select(grouping => new {Player = grouping.Key, Total = grouping.Select(arg => arg.Player2).Distinct().Count()});

                Log.Logger.Information("");
                foreach (var item in summary2.OrderByDescending(arg => arg.Total))
                {
                    Log.Logger.Information($"{item.Player,-20} {item.Total,3}");
                }

                // All streaks
                var players = eloGames.SelectMany(game => new[] {game.Game.Player1.EmailAddress, game.Game.Player2.EmailAddress}).Distinct().ToList();

                var list = new Dictionary<string, dynamic>();
                foreach (var player in players)
                {
                    var streaks = statsService.WinningStreaks(player, DateTime.MinValue, DateTime.MaxValue);
                    var totalWins = streaks.Sum(s => s.Count());
                    if (streaks.Count() == 0)
                        continue;

                    var countAvg = ((decimal) streaks.Sum(s => s.Count()) / streaks.Count()).Round(1);
                    var eloAvg = ((decimal) streaks.Sum(s => s.Sum(game => game.Delta1)) / streaks.Count()).Round(1);
                    list.Add(player, new { Count= streaks.Count(), Avg = countAvg, EloAvg = eloAvg});
                }

                Console.WriteLine("");
                Console.WriteLine("Winner streaks");
                foreach (var item in list.OrderByDescending(pair => pair.Value.Count))
                {
                    Console.WriteLine($"{item.Key, -35} {item.Value.Count,4} {item.Value.Avg, 4} {item.Value.EloAvg, 4}");
                }
                
                // losing streaks
                var list2 = new Dictionary<string, dynamic>();
                foreach (var player in players)
                {
                    var streaks = statsService.LosingStreaks(player, DateTime.MinValue, DateTime.MaxValue);
                    if (streaks.Count() < 4)
                        continue;

                    var countAvg = ((decimal) streaks.Sum(s => s.Count()) / streaks.Count()).Round(1);
                    var eloAvg = ((decimal) streaks.Sum(s => s.Sum(game => game.Delta1)) / streaks.Count()).Round(1);
                    list2.Add(player, new { Count = streaks.Count(), Percentage = countAvg, EloAvg = eloAvg });
                }

                Console.WriteLine("");
                Console.WriteLine("Loser streaks");
                foreach (var item in list2.OrderByDescending(pair => pair.Value.Count))
                {
                    Console.WriteLine($"{item.Key, -35} {item.Value.Count,4} {item.Value.Percentage, 4} {item.Value.EloAvg, 4}");
                }
           
                // var result = eloGames.SelectMany((game, i) => new List<EloGame>()
                // {
                //     game, new EloGame(new Game()
                //     {
                //         GameType = game.Game.GameType,
                //         Player1 = game.Game.Player2,
                //         Player2 = game.Game.Player1,
                //         Score1 = game.Game.Score2,
                //         Score2 = game.Game.Score1,
                //         RegistrationDate = game.Game.RegistrationDate,
                //         Venue = game.Game.Venue,
                //     }, game.EloPlayer2, game.EloPlayer1, game.Player2Delta, game.Player1Delta)
                // }).GroupBy(game => game.Game.Player1.EmailAddress);
                //
                // var numberOfPlayers = result.Count();
                //
                // foreach (var item in result)
                // {
                //     eloGames = item.ToList();
                //     var op = eloGames.Select(game => game.Game.Player2.EmailAddress).Distinct().Count();
                //
                //     var groups = eloGames.GroupBy(game => game.Game.Player2.EmailAddress)
                //         .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList().Count);
                //
                //     double total = 0;
                //     var avg = eloGames.Count() / numberOfPlayers;
                //
                //     foreach (var g in groups)
                //     {
                //         total += Math.Pow(g.Value - avg, 2);
                //     }
                //
                //     var r = Math.Sqrt(total / numberOfPlayers);
                //
                //     Console.WriteLine(
                //         $"{item.Key,-25}: {eloGames.Count(),5} {op,3} {(100 * op) / numberOfPlayers,3} {r}");
                // }
                // // foreach (var game in games)
                // {
                //     gamesService.RegisterGame(new Game()
                //     {
                //         Player1 = game.Player1,
                //         Player2 = game.Player2,
                //         Venue = game.Venue,
                //         GameType = game.GameType,
                //         Score1 = game.Score1,
                //         Score2 = game.Score2,
                //         RegistrationDate = DateTime.Now
                //     });
                // }


                // var calculator = new EloCalculatorVersion2020();

                // var x = calculator.CalculateDeltaPlayer(1220, 2000, 2, 3).Round(3);
                // var y = calculator.CalculateDeltaPlayer(1220, 2000, 1, 3).Round(3);
                // var z = calculator.CalculateDeltaPlayer(1220, 2000, 0, 3).Round(3);

                // Console.WriteLine($"{x} == {y} == {z}");

                // x = calculator.CalculateDeltaPlayer(1200, 1200, 3, 2).Round(3);
                // y = calculator.CalculateDeltaPlayer(1200, 1200, 3, 1).Round(3);
                // z = calculator.CalculateDeltaPlayer(1200, 1200, 3, 0).Round(3);

                // Console.WriteLine($"{x} == {y} == {z}");
                //TestStats();
                //ShowMatrix();

                //var statsService = CreateStatisticsService();
                Console.WriteLine("Ready");
                Console.ReadLine();
            }

            private static void ShowMatrix()
            {
                var pi = (decimal) Math.PI / 2;

                foreach (var x in Enumerable.Range(0, 50))
                {
                    var delta = (decimal) Math.Atan((double) x);
                    Console.WriteLine($"delta({x}) = {delta}");
                }

                Console.WriteLine("...............");

                var calculator = new EloCalculatorVersion2020();
                var range1 = new List<int>() {1200, 1500, 1700}; //Enumerable.Range(12, 6);
                var range2 = new List<int>() {1300, 1705, 2000}; //Enumerable.Range(12, 6);
                foreach (var r1 in range1)
                {
                    foreach (var r2 in range2)
                    {
                        NewMethod(calculator, r1, r2, 3, 0);
                        NewMethod(calculator, r1, r2, 3, 1);
                        NewMethod(calculator, r1, r2, 3, 2);
                        NewMethod(calculator, r1, r2, 2, 0);
                        NewMethod(calculator, r1, r2, 2, 1);
                        NewMethod(calculator, r1, r2, 1, 0);
                    }

                    //Console.WriteLine(".................");
                    //foreach (var r2 in range2)
                    //{
                    //    NewMethod(calculator, r1, r2, 2, 0);
                    //}

                    //Console.WriteLine(".................");
                    //foreach (var r2 in range2)
                    //{
                    //    NewMethod(calculator, r1, r2, 3, 0);
                    //}


                    //  Console.WriteLine(".................");

                    //  foreach (var r2 in range2)
                    //  {
                    //      ShowResults2(calculator, r1, r2);
                    //  }

//                    Console.WriteLine(".................");
//                    foreach (var r2 in range2)
//                    {
//                        ShowResults3(calculator, r1, r2);
//                    }
                }
            }


            private static void ShowResults(IEloCalculator calculator, in int r1, in int r2, int score1, int score2)
            {
                NewMethod(calculator, r1, r2, 1, 0);
            }

            private static void ShowResults2(IEloCalculator calculator, in int r1, in int r2)
            {
                NewMethod(calculator, r1, r2, 1, 0);
                NewMethod(calculator, r1, r2, 2, 0);
                NewMethod(calculator, r1, r2, 3, 0);
                NewMethod(calculator, r1, r2, 4, 0);
                NewMethod(calculator, r1, r2, 5, 0);
                NewMethod(calculator, r1, r2, 6, 0);
            }

            private static void ShowResults3(IEloCalculator calculator, in int r1, in int r2)
            {
                NewMethod(calculator, r1, r2, 1, 0);
                NewMethod(calculator, r1, r2, 2, 1);
                NewMethod(calculator, r1, r2, 2, 0);
                NewMethod(calculator, r1, r2, 3, 1);
                NewMethod(calculator, r1, r2, 3, 2);
                NewMethod(calculator, r1, r2, 3, 0);
            }

            private static void NewMethod(IEloCalculator calculator, int r1, int r2, int score1, int score2)
            {
                var result1 = calculator.CalculateDeltaPlayer(r1, r2, score1, score2).Round(2);
                var result2 = calculator.CalculateDeltaPlayer(r2, r1, score2, score1).Round(2);

                var result1b = calculator.CalculateDeltaPlayer(r1, r2, score2, score1).Round(2);
                var result2b = calculator.CalculateDeltaPlayer(r2, r1, score1, score2).Round(2);

                var expectedOutcome1 = GeneralEloCalculator.CalculateExpectationForBestOf(r1, r2, score1 + score2)
                    .Round(2);

                Console.WriteLine($"{r1}-{r2} {score1}-{score2} {result1,3}:{result2,3} ==== {score2}-{score1} {result1b,3}:{result2b,3}");
            }

            private static void TestStats()
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

                    Console.WriteLine(
                        $"{item.Key,-25}: {eloGames.Count,5} {op,3} {(100 * op) / numberOfPlayers,3} {r}");
                }
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