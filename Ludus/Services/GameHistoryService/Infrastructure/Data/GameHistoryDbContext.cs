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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameHistory>(eb =>
            {
                eb.ToTable("GameHistories");
                eb.HasKey(x => x.MatchId);
                eb.Property(x => x.MatchId).IsRequired();
                eb.Property(x => x.PlayerUserIds).HasColumnType("text[]").IsRequired();
                eb.HasIndex(x => x.PlayerUserIds).HasMethod("GIN");
                eb.Property(x => x.MoveHistory).IsRequired();
                eb.Property(x => x.WinnerUserId);
                eb.Property(x => x.StartedAt).HasColumnType("timestamp with time zone").IsRequired();
                eb.Property(x => x.EndedAt).HasColumnType("timestamp with time zone").IsRequired();
                eb.HasMany(x => x.ChatMessages).WithOne(x => x.GameHistory).HasForeignKey(x => x.GameMatchId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(eb =>
            {
                eb.ToTable("ChatMessages");
                eb.HasKey(x => x.Id);
                eb.Property<Guid>("Id").IsRequired().HasColumnType("uuid");
                eb.Property(x => x.SenderUserId).IsRequired();
                eb.Property(x => x.Message).IsRequired();
                eb.Property(x => x.SentAt).IsRequired().HasColumnType("timestamp with time zone");
                eb.Property(x => x.GameMatchId).IsRequired();
                eb.HasIndex(x => new { x.GameMatchId, x.SenderUserId, x.SentAt, x.Message }).IsUnique();
            });
        }
    }
}

