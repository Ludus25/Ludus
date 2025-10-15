namespace Common.Dto
{
    public record GameEndedEvent(string MatchId, List<string> PlayerUserIds, DateTime StartedAt, DateTime EndedAt, string MoveHistory, string? WinnerUserId);
}
