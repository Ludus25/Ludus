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
            // Mapiranje klase Message na tabelu "messages"
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
        }
    }
}
