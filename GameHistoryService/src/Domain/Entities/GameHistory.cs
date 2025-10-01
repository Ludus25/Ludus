public class GameHistory
{
    public string MatchId { get; set; }
    public List<string> PlayerUserIds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public string MoveHistory { get; set; }

    public List<ChatMessage> ChatMessages { get; set; }
}
