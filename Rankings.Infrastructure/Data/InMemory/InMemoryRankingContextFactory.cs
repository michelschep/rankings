using Microsoft.EntityFrameworkCore;

namespace Rankings.Infrastructure.Data.InMemory
{
    public class InMemoryRankingContextFactory 
    {
        public RankingContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);

            var rankingContext = new InMemoryRankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureDeleted();
            rankingContext.Database.EnsureCreated();

            return rankingContext;
        }
    }
}