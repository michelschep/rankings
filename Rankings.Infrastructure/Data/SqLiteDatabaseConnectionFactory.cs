using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data
{
    public class SqLiteDatabaseConnectionFactory : ISqLiteConnectionFactory
    {
        public SqliteConnection CreateSqliteConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}