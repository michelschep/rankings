using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteRankingContextFactory :IRankingContextFactory 
    {
        private readonly IConfiguration _configuration;

        // Needed for migrations
        public SqLiteRankingContextFactory()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
        }

        // ReSharper disable once EmptyConstructor
        public SqLiteRankingContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public RankingContext CreateDbContext(string[] args)
        {
            return CreateDbContext1();
        }

        public RankingContext CreateDbContext1()
        {
            var repositoryConfiguration = new RepositoryConfiguration();
            _configuration.GetSection("Repository").Bind(repositoryConfiguration);

            return CreateDbContext2(repositoryConfiguration.Database);
        }

        private RankingContext CreateDbContext2(string database)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            var connection = new SqliteConnection(database);
            optionsBuilder.UseSqlite(connection);

            return new RankingContext(optionsBuilder.Options);
        }
    }
}