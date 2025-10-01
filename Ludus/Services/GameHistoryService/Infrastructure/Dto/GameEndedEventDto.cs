namespace Dto
{
    public class GameEndedEventDto
    {
        public string MatchId { get; set; } = string.Empty;
        public List<string> PlayerUserIds { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string MoveHistory { get; set; } = string.Empty;
        public string? WinnerUserId { get; set; }
    }
}
