using System;
using Rankings.Core.Interfaces;

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
            var context = _rankingContextFactory.Create(connectionString);

            return new EfRepository(context);
        }
    }
}