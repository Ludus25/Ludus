namespace Common.Dto
{
    public record GameEndedEvent(string MatchId, List<string> PlayerUserIds, List<string> PlayerEmails, DateTime StartedAt, DateTime EndedAt, string MoveHistory, string? WinnerUserId);
}
