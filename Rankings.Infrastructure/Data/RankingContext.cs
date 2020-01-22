using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Rankings.Core.Entities;

namespace Rankings.Infrastructure.Data
{
    public abstract class RankingContext : DbContext
    {
        protected RankingContext(DbContextOptions options) : base(options)
        {
        }

        // EF needs these properties!
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Profile> Profiles { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<GameType> GameTypes { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Game> Games { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Venue> Venues { get; set; }
    }

    public sealed class PersistantRankingContext : RankingContext
    {
        public PersistantRankingContext(DbContextOptions<RankingContext> options) : base(options)
        {
            Database.Migrate();
        }
    }

    public sealed class InMemoryRankingContext : RankingContext
    {
        public InMemoryRankingContext(DbContextOptions<RankingContext> options) : base(options)
        {
        }
    }
}