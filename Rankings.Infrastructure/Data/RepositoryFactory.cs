﻿using System;
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
            var context = _rankingContextFactory.CreateDbContext(args: null);
            context.Database.Migrate();

            return new EfRepository(context);
        }
    }
}