using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteRankingContextFactory : IRankingContextFactory
    {
        private readonly ISqLiteConnectionFactory _connectionFactory;

        private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name
                    && level == LogLevel.Information)
                .AddConsole();
        });

        public SqLiteRankingContextFactory(ISqLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public RankingContext Create(string connectionString)
        {
            var connection = _connectionFactory.CreateSqliteConnection(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseSqlite(connection);
//            optionsBuilder.UseLoggerFactory(MyLoggerFactory);

            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureCreated(); // TODO this is on two places now. WHy??

            return rankingContext;
        }
    }
}