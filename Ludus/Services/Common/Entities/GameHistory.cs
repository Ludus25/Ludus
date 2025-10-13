using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Common.Entities
{
    public class GameHistory
    {
        public string MatchId { get; set; } = default!;
        public List<string> PlayerUserIds { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string MoveHistory { get; set; } = default!;
        public string? WinnerUserId { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; } = new Collection<ChatMessage>();
    }
}

