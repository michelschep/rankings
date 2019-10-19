using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteInMemoryConnectionFactory : ISqLiteConnectionFactory
    {
        public DbConnection CreateSqliteConnection(string connectionString)
        {
            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            return sqliteConnection;
        }
    }
}