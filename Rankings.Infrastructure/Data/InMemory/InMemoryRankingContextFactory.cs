using Microsoft.EntityFrameworkCore;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.Infrastructure.Data.InMemory
{
    public class InMemoryRankingContextFactory : IRankingContextFactory
    {
        public RankingContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);

            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureDeleted();
            rankingContext.Database.EnsureCreated();

            return rankingContext;
        }

        public RankingContext CreateDbContext(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}