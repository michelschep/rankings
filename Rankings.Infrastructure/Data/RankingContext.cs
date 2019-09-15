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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>().ToTable("Profile");
            modelBuilder.Entity<GameType>().ToTable("GameType");
        }
    }
}