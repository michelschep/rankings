using Microsoft.EntityFrameworkCore;
using Rankings.Core.Entities;

namespace Rankings.Infrastructure.Data
{
    public class RankingContext : DbContext
    {
        public RankingContext(DbContextOptions<RankingContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Venue> Venues { get; set; }
    }
}