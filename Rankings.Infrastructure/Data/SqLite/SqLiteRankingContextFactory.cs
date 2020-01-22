using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rankings.Infrastructure.Data.SqLite
{
    public interface IRankingContextFactory : IDesignTimeDbContextFactory<RankingContext>
    {

    }

    public class SqLiteRankingContextFactory :IRankingContextFactory 
    {
        //private readonly ISqLiteConnectionFactory _connectionFactory;
        //private readonly ILoggerFactory _loggerFactory;
        //private readonly RepositoryConfiguration _repositoryConfiguration;

        public SqLiteRankingContextFactory()
        {
            
        }
        public SqLiteRankingContextFactory(ISqLiteConnectionFactory connectionFactory, ILoggerFactory loggerFactory, RepositoryConfiguration repositoryConfiguration)
        {
            //_connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            //_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            //_repositoryConfiguration = repositoryConfiguration ?? throw new ArgumentNullException(nameof(repositoryConfiguration));
        }

        public RankingContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            var repositoryConfiguration = new RepositoryConfiguration();
            config.GetSection("Repository").Bind(repositoryConfiguration);
            var connection = new SqliteConnection(repositoryConfiguration.Database); 
            optionsBuilder.UseSqlite(connection);
            //optionsBuilder.UseLoggerFactory(_loggerFactory);

            var rankingContext = new PersistantRankingContext(optionsBuilder.Options);
            //rankingContext.Database.EnsureCreated(); // TODO this is on two places now. WHy??

            return rankingContext;
        }
    }
}