using System;
using System.IO;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Rankings.Core.Entities;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;

namespace EventStoreTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            IRankingContextFactory contextFactory = new SqLiteRankingContextFactory(configuration);
            var repoFactory = new RepositoryFactory(contextFactory);
            var repository = repoFactory.Create();

            var games = repository.List<Game>();

            using (var db = new LiteDatabase(@"C:\DATA\Temp\MyData.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Game>("games");

                foreach (var game in games)
                {
                    
                    //col.Insert(game);
                    //Console.WriteLine(game.Player1Id);
                    //col.Update(game);
                }

                // Use LINQ to query documents (filter, sort, transform)
                var results = col.Query()
                    //.Where(x => x.Player1.DisplayName.Equals("Postma"))
                    .ToList();

                foreach (var result in results)
                {
                    //Console.WriteLine($"{result.Player1Id} {result.RegistrationDate}");
                }

                Console.WriteLine(results.Count);
            }


            

            Console.ReadLine();
        }
    }
}
