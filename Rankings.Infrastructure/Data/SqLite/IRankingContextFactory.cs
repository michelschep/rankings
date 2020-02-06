using Microsoft.EntityFrameworkCore.Design;

namespace Rankings.Infrastructure.Data.SqLite
{
    public interface IRankingContextFactory : IDesignTimeDbContextFactory<RankingContext>
    {

    }
}