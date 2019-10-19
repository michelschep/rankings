using System.Data.Common;

namespace Rankings.Infrastructure.Data.SqLite
{
    public interface ISqLiteConnectionFactory
    {
        DbConnection CreateSqliteConnection(string connectionString);
    }
}