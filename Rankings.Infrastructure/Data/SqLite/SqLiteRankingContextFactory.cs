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

        // ReSharper disable once EmptyConstructor
        public SqLiteRankingContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RankingContext CreateDbContext(string[] args)
        {
            // var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //
            // IConfiguration config = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{environment}.json", optional: true)
            // .AddJsonFile($"secrets.json", optional: true)
            // .AddEnvironmentVariables()
            // .Build();

            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            var repositoryConfiguration = new RepositoryConfiguration();
            _configuration.GetSection("Repository").Bind(repositoryConfiguration);
            var connection = new SqliteConnection(repositoryConfiguration.Database); 
            optionsBuilder.UseSqlite(connection);

            return new RankingContext(optionsBuilder.Options);
        }
    }
}