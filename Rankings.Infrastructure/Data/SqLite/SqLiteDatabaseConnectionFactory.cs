using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteDatabaseConnectionFactory : ISqLiteConnectionFactory
    {
        public DbConnection CreateSqliteConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}