using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Rankings.Core.Entities;
using Rankings.Core.Services;
using Rankings.Core.Services.ToBeObsolete;
using Rankings.Core.Specifications;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var statsService = CreateStatisticsService();
            //var lastPointInTime = statsService.CalculateStats(DateTime.MinValue, DateTime.MaxValue);

            var ranking = statsService.Ranking("tafeltennis", DateTime.MinValue, DateTime.MaxValue);
            var index = 1;
            foreach (var item in ranking.DeprecatedRatings.OrderByDescending(pair => pair.Value.Ranking).ThenBy(pair => pair.Key.DisplayName))
            {
                Console.WriteLine($"|{index++}|{item.Key.DisplayName}|{item.Value.Ranking.Round(0)}|");
            }
//            foreach (var item in lastPointInTime.Value.NewPlayerStats.OrderByDescending(pair => pair.Value.Rating))
//            {
//                var playerStats = item.Value;
//                var timespan = new TimeSpan(0, playerStats.TimeNumberOne, 0);
//                //var meanEloWon = (int)(playerStats.EloWonOpponent / (playerStats.Won + 0.00001m));
//                //var meanEloLost = (int)(playerStats.EloLostOpponent / (playerStats.Lost + 0.00001m));
//                //var meanEloTotal = (int)((playerStats.EloLostOpponent + playerStats.EloWonOpponent) /
//                //                         (playerStats.NumberOfGames + 0.00001m));
//                // {100*playerStats.NumberOfCleanSheets/playerStats.NumberOfGames, 5}
//                Console.WriteLine($"\t{item.Key.DisplayName,-20} {playerStats.Rating,5} {playerStats.WonStreakRecord,3} {playerStats.WonStreak,3} {playerStats.GoatRating,5}  {timespan,15} ");
//                //Console.WriteLine($"\t{item.Key.DisplayName, -20} {playerStats.Rating, 5} {playerStats.WonStreakRecord, 3} {playerStats.WonStreak, 3} {playerStats.GoatRating, 5} {meanEloTotal, 5} {meanEloWon, 5} {meanEloLost, 5} {timespan, 15} ");
//            }

            var rankingService = CreateGamesService();
//
//            foreach (var r in rankingService.List<Game>(new GamesForPeriodSpecification("tafeltennis", DateTime.MinValue, DateTime.MaxValue)))
//            {
//                var rd = String.Format("{0:yyyy-MM-dd HH:mm}", r.RegistrationDate);
//                var p1 = String.Format("{0, -10}", r.Player1.DisplayName);
//                var p2 = String.Format("{0, -10}", r.Player2.DisplayName);
//                Console.WriteLine("| {0} | {1} | {2} | {3} | {4} |"
//                    , rd
//                    , p1
//                    , p2
//                    , r.Score1
//                    , r.Score2
//                    );
//            }
            Console.ReadLine();
        }

        private static StatisticsService CreateStatisticsService()
        {
            var rankingService = CreateGamesService();

            EloConfiguration eloConfiguration = new EloConfiguration(50, 400, true, 1200);
            var statsService = new StatisticsService(rankingService, eloConfiguration, null, new EloCalculator(eloConfiguration, null));
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
