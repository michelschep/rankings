using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteRankingContextFactory : IRankingContextFactory
    {
        private readonly ISqLiteConnectionFactory _connectionFactory;
        private readonly ILoggerFactory _loggerFactory;

        //private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder
        //        .AddFilter((category, level) =>
        //            category == DbLoggerCategory.Database.Command.Name
        //            && level == LogLevel.Information)
        //        .AddConsole();
        //});

        public SqLiteRankingContextFactory(ISqLiteConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public RankingContext Create(string connectionString)
        {
            var connection = _connectionFactory.CreateSqliteConnection(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseSqlite(connection);
            optionsBuilder.UseLoggerFactory(_loggerFactory);

            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureCreated(); // TODO this is on two places now. WHy??

            return rankingContext;
        }
    }
}