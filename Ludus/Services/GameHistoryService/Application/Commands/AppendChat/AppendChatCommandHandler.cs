using Interfaces;
using MediatR;

namespace Commands
{
    public class AppendChatCommandHandler : IRequestHandler<AppendChatCommand>
    {
        private readonly IGameHistoryRepository _repo;

        public AppendChatCommandHandler(IGameHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AppendChatCommand request, CancellationToken ct)
        {
            foreach (var m in request.Messages)
            {
                m.GameMatchId = request.MatchId;
            }

            await _repo.AppendChatMessageAsync(request.MatchId, request.Messages, ct);
        }
    }
}
