using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Rankings.Core.Entities;

namespace Rankings.Infrastructure.Data
{
    public sealed class RankingContext : DbContext
    {
        public RankingContext(DbContextOptions options) : base(options)
        {
        }

        // EF needs these properties!
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Profile> Profiles { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<GameType> GameTypes { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Game> Games { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<DoubleGame> DoubleGames { get; set; }
        [SuppressMessage("ReSharper", "UnusedMember.Global")] public DbSet<Venue> Venues { get; set; }
    }
}