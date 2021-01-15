using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public IRepository Create()
        {
            var context = _rankingContextFactory.CreateDbContext1();
            context.Database.Migrate();
            //
            // foreach (var g in context.Games.ToList())
            // {
            //     g.Identifier = Guid.NewGuid().ToString();
            // }
            //
            // context.SaveChanges();


            return new EfRepository(context);
        }
    }
}