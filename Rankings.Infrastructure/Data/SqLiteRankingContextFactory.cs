using System;
using Microsoft.Data.Sqlite;
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
            rankingContext.Database.EnsureCreated();

            return rankingContext;
        }
    }

    public interface ISqLiteConnectionFactory
    {
        SqliteConnection CreateSqliteConnection(string connectionString);
    }

    public class SqLiteInMemoryConnectionFactory : ISqLiteConnectionFactory
    {
        public SqliteConnection CreateSqliteConnection(string connectionString)
        {
            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            return sqliteConnection;
        }
    }

    public class SqLiteDatabaseConnectionFactory : ISqLiteConnectionFactory
    {
        public SqliteConnection CreateSqliteConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}