using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class GameHistoryDbContext : DbContext
    {
        public GameHistoryDbContext(DbContextOptions<GameHistoryDbContext> options) : base(options)
        { }

        public DbSet<GameHistory> GameHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameHistory>(eb =>
            {
                eb.ToTable("GameHistories");
                eb.HasKey(x => x.MatchId);
                eb.Property(x => x.MatchId).IsRequired();
                eb.Property(x => x.PlayerUserIds).HasColumnType("text[]").IsRequired();
                eb.HasIndex(x => x.PlayerUserIds).HasMethod("GIN");
                eb.Property(x => x.PlayerEmails).HasColumnType("text[]").IsRequired();
                eb.HasIndex(x => x.PlayerEmails).HasMethod("GIN");
                eb.Property(x => x.MoveHistory).IsRequired();
                eb.Property(x => x.WinnerUserId);
                eb.Property(x => x.StartedAt).HasColumnType("timestamp with time zone").IsRequired();
                eb.Property(x => x.EndedAt).HasColumnType("timestamp with time zone").IsRequired();
            });
        }
    }
}

