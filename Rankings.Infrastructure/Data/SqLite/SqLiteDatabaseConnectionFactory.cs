using System.Data;
using Microsoft.Data.Sqlite;

namespace Rankings.Infrastructure.Data.SqLite
{
    public class SqLiteDatabaseConnectionFactory : ISqLiteConnectionFactory
    {
        public IDbConnection CreateSqliteConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}