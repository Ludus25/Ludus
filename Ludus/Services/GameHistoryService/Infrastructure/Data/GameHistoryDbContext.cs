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
                eb.Property(x => x.MatchId).IsRequired().HasColumnType("text");
                eb.Property(x => x.PlayerUserIds).HasColumnType("text[]").IsRequired();
                eb.HasIndex(x => x.PlayerUserIds).HasMethod("GIN");
                eb.Property(x => x.MoveHistory).HasColumnType("text");
                eb.Property(x => x.WinnerUserId).HasColumnType("text");
                eb.Property(x => x.StartedAt).HasColumnType("timestamp with time zone");
                eb.Property(x => x.EndedAt).HasColumnType("timestamp with time zone");
                eb.HasMany(x => x.ChatMessages).WithOne(x => x.GameHistory).HasForeignKey("GameMatchId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(eb =>
            {
                eb.ToTable("ChatMessages");
                eb.HasKey("Id");
                eb.Property<Guid>("Id").HasColumnType("uuid");
                eb.Property(x => x.SenderUserId).IsRequired().HasColumnType("text");
                eb.Property(x => x.Message).HasColumnType("text");
                eb.Property(x => x.SentAt).HasColumnType("timestamp with time zone");
                eb.Property(x => x.GameMatchId).HasColumnType("text");
            });
        }
    }
}

