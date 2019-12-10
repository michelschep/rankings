using System;
using Microsoft.EntityFrameworkCore;

namespace Rankings.Infrastructure.Data.InMemory
{
    public class InMemoryRankingContextFactory : IRankingContextFactory
    {
        public RankingContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RankingContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);

            var rankingContext = new RankingContext(optionsBuilder.Options);
            rankingContext.Database.EnsureDeleted();
            rankingContext.Database.EnsureCreated();

            return rankingContext;
        }
    }
}