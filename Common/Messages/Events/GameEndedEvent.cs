public class GameEndedEvent
{
    public string MatchId { get; set; }
    public List<string> PlayerUserIds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public Dictionary<string, int> Scores { get; set; }
    public string MoveHistory { get; set; }
}
