using ChatService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .ToTable("messages")
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .Property(m => m.Sender)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Message>()
                .Property(m => m.Content)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Message>()
                .Property(m => m.GameId)
                .IsRequired()
                .HasMaxLength(50); 
        }
    }
}
