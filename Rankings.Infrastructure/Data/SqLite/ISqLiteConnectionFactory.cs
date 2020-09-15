using System.Data;

namespace Rankings.Infrastructure.Data.SqLite
{
    public interface ISqLiteConnectionFactory
    {
        IDbConnection CreateSqliteConnection(string connectionString);
    }
}