using Microsoft.EntityFrameworkCore;

namespace Rankings.Infrastructure.Data.InMemory
{
    public class InMemoryRankingContextFactory 
    {
        public RankingContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);

            return new InMemoryRankingContext(optionsBuilder.Options);
        }
    }
}