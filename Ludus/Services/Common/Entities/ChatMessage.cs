using System.Text.Json.Serialization;

namespace Common.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string SenderUserId { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime SentAt { get; set; }

        public required string GameMatchId { get; set; }

        [JsonIgnore]
        public GameHistory? GameHistory { get; set; }
    }
}
