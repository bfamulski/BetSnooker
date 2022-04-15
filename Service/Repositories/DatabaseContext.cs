using BetSnooker.Models;
using BetSnooker.Models.API;
using Microsoft.EntityFrameworkCore;

namespace BetSnooker.Repositories
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RoundBets> RoundBets { get; set; }

        public DbSet<Bet> MatchBets { get; set; }

        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoundInfo>().HasKey(r => r.Round);
        }
    }
}