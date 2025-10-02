using Common.Entities;

namespace Common.Dto
{
    public class ChatLogEventDto
    {
        public string MatchId { get; set; } = string.Empty;
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
