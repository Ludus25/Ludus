using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class GameHistoryDbContext : DbContext
    {
        public GameHistoryDbContext(DbContextOptions<GameHistoryDbContext> options) : base(options)
        { }

        public DbSet<GameHistory> GameHistories { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameHistory>(eb =>
            {
                eb.HasKey(x => x.MatchId);
                eb.Property(x => x.MatchId).IsRequired();
                eb.Property(x => x.MoveHistory).HasColumnType("text");
                eb.Property(x => x.PlayerUserIds).HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).HasColumnName("PlayerUserIds");
                eb.HasMany(x => x.ChatMessages).WithOne().HasForeignKey("GameMatchId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(eb =>
            {
                eb.HasKey("Id");
                eb.Property<Guid>("Id").HasColumnType("uuid");
                eb.Property(x => x.SenderUserId).IsRequired();
                eb.Property(x => x.Message).HasColumnType("text");
            });
        }
    }
}

