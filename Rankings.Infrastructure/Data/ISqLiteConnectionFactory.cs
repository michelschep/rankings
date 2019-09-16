using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data
{
    public interface ISqLiteConnectionFactory
    {
        SqliteConnection CreateSqliteConnection(string connectionString);
    }
}