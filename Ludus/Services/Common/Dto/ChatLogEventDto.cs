using Common.Entities;

namespace Common.Dto
{
    public record ChatLogEvent(string MatchId, List<ChatMessage> Messages);
}
