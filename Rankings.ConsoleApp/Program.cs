using System;
using System.Collections.Generic;
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

            DateTime currentDate = DateTime.MinValue;

            var days = new Dictionary<string, int>();
            var games = new Dictionary<string, int>();
            var minutes = new Dictionary<string, double>();

            var previousNumberOne = "";
            var allGames = rankingService.Games().ToList();
            DateTime previousGame = allGames.First().RegistrationDate;
            foreach (var game in allGames)
            {
                var ranking = rankingService.Ranking("tafeltennis", game.RegistrationDate);
                var keyValuePair = ranking.OldRatings.OrderByDescending(pair => pair.Value.Ranking).First();

                if (!games.ContainsKey(keyValuePair.Key.DisplayName))
                    games[keyValuePair.Key.DisplayName] = 0;

                games[keyValuePair.Key.DisplayName] += 1;

                if (!minutes.ContainsKey(keyValuePair.Key.DisplayName))
                    minutes[keyValuePair.Key.DisplayName] = 0;

                var gameRegistrationDate = (game.RegistrationDate - previousGame);
                var delta = gameRegistrationDate.TotalMinutes;
                if (previousNumberOne != "")
                    minutes[previousNumberOne] += delta;

                if (game.RegistrationDate.Date > currentDate)
                {
                    currentDate = game.RegistrationDate;
                    Console.WriteLine($"\t{keyValuePair.Key.DisplayName}: {keyValuePair.Value.Ranking}");
                    if (!days.ContainsKey(keyValuePair.Key.DisplayName))
                        days[keyValuePair.Key.DisplayName] = 0;

                    days[keyValuePair.Key.DisplayName] += 1;
                    Console.WriteLine($"Date: {currentDate}");
                }

                Console.WriteLine($"==>{keyValuePair.Key.DisplayName}: {keyValuePair.Value.Ranking}");

                previousNumberOne = keyValuePair.Key.DisplayName;
                previousGame = game.RegistrationDate;
            }

            foreach (var day in days)
            {
                Console.WriteLine($"DAY {day.Key}: {day.Value}");
            }

            foreach (var game in games)
            {
                Console.WriteLine($"GAME {game.Key}: {game.Value}");
            }

            foreach (var game in minutes)
            {
                Console.WriteLine($"MINUTES {game.Key}: {game.Value}");
            }
            Console.ReadLine();
        }
    }
}
