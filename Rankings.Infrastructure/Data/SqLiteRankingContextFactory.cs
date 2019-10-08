using System;
using Microsoft.EntityFrameworkCore;

namespace Rankings.Infrastructure.Data
{
    public class SqLiteRankingContextFactory : IRankingContextFactory
    {
        private readonly ISqLiteConnectionFactory _connectionFactory;

        public SqLiteRankingContextFactory(ISqLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public RankingContext Create(string connectionString)
        {
            var connection = _connectionFactory.CreateSqliteConnection(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseSqlite(connection);
            
            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureCreated(); // TODO this is on two places now. WHy??

            return rankingContext;
        }
    }

    public class InMemoryRankingContextFactory : IRankingContextFactory
    {
        public RankingContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);

            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureCreated();

            return rankingContext;
        }
    }
}