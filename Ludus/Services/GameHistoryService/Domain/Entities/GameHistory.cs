using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class GameHistory
    {
        [Key]
        public string MatchId { get; set; }
        public List<string> PlayerUserIds { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string MoveHistory { get; set; }
        public string? WinnerUserId { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
    }

}

