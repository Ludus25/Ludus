using Common.Entities;
using MediatR;

namespace Commands
{
    public record AppendChatCommand(string MatchId, IEnumerable<ChatMessage> Messages) : IRequest;
}
