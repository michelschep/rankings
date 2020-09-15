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
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            var repositoryConfiguration = new RepositoryConfiguration();
            _configuration.GetSection("Repository").Bind(repositoryConfiguration);
            var connection = new SqliteConnection(repositoryConfiguration.Database); 
            optionsBuilder.UseSqlite(connection);

            return new RankingContext(optionsBuilder.Options);
        }
    }
}