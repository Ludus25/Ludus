using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SenderUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public string? GameMatchId { get; set; }
    }
}
