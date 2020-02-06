using System;
using Rankings.Core.Interfaces;

namespace Rankings.Infrastructure.Data.InMemory
{
    public class InMemoryRepositoryFactory
    {
        private readonly InMemoryRankingContextFactory _rankingContextFactory;

        public InMemoryRepositoryFactory(InMemoryRankingContextFactory rankingContextFactory)
        {
            _rankingContextFactory = rankingContextFactory ?? throw new ArgumentNullException(nameof(rankingContextFactory));
        }

        public IRepository Create(string connectionString)
        {
            var context = _rankingContextFactory.CreateDbContext(connectionString);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return new EfRepository(context);
        }
    }
}