using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data
{
    public class SqLiteInMemoryConnectionFactory : ISqLiteConnectionFactory
    {
        public SqliteConnection CreateSqliteConnection(string connectionString)
        {
            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            return sqliteConnection;
        }
    }
}