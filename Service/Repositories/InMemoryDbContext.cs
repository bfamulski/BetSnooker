using BetSnooker.Models;
using BetSnooker.Models.API;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public class InMemoryDbContext : DbContext
    {
        public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RoundBets> RoundBets { get; set; }

        public DbSet<Bet> MatchBets { get; set; }

        public DbSet<Score> Scores { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoundBets>()
                .HasMany(b => b.MatchBets)
                .WithOne();
        }
    }
}