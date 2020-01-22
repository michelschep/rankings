using System;
using Rankings.Core.Interfaces;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.Infrastructure.Data
{
    public class RepositoryFactory
    {
        private readonly IRankingContextFactory _rankingContextFactory;

        public RepositoryFactory(IRankingContextFactory rankingContextFactory)
        {
            _rankingContextFactory = rankingContextFactory ?? throw new ArgumentNullException(nameof(rankingContextFactory));
        }

        public IRepository Create(string connectionString)
        {
            // TODO why null?
            var context = _rankingContextFactory.CreateDbContext(null);

            return new EfRepository(context);
        }
    }
}