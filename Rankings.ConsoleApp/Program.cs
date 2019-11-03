using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var statsService = CreateStatisticsService();
            var lastPointInTime = statsService.CalculateStats();

            Console.WriteLine("");
            foreach (var item in lastPointInTime.Value.NewPlayerStats.OrderByDescending(pair => pair.Value.Rating))
            {
                var playerStats = item.Value;
                var timespan = new TimeSpan(0, playerStats.TimeNumberOne, 0);
                var meanEloWon = (int)(playerStats.EloWonOpponent / (playerStats.Won + 0.00001m));
                var meanEloLost = (int)(playerStats.EloLostOpponent / (playerStats.Lost + 0.00001m));
                var meanEloTotal = (int)((playerStats.EloLostOpponent + playerStats.EloWonOpponent) /
                                         (playerStats.NumberOfGames + 0.00001m));
                // {100*playerStats.NumberOfCleanSheets/playerStats.NumberOfGames, 5}
                Console.WriteLine($"\t{item.Key.DisplayName,-20} {playerStats.Rating,5} {playerStats.WonStreakRecord,3} {playerStats.WonStreak,3} {playerStats.GoatRating,5}  {timespan,15} ");
                //Console.WriteLine($"\t{item.Key.DisplayName, -20} {playerStats.Rating, 5} {playerStats.WonStreakRecord, 3} {playerStats.WonStreak, 3} {playerStats.GoatRating, 5} {meanEloTotal, 5} {meanEloWon, 5} {meanEloLost, 5} {timespan, 15} ");
            }

            Console.ReadLine();
        }

        private static StatisticsService CreateStatisticsService()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .Build();

            var database = config["Database"];

            var connectionFactory = new SqLiteDatabaseConnectionFactory();
            var sqLiteRankingContextFactory = new SqLiteRankingContextFactory(connectionFactory);
            var repositoryFactory = new RepositoryFactory(sqLiteRankingContextFactory);

            var repository = repositoryFactory.Create(database);
            var rankingService = new RankingService(repository);
            var statsService = new StatisticsService(rankingService);
            return statsService;
        }
    }
}
