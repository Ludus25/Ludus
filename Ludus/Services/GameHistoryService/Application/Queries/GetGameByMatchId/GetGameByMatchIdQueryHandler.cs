using Common.Entities;
using Interfaces;
using MediatR;

namespace Queries
{
    public class GetGameByMatchIdQueryHandler : IRequestHandler<GetGameByMatchIdQuery, GameHistory?>
    {
        private readonly IGameHistoryRepository _repo;

        public GetGameByMatchIdQueryHandler(IGameHistoryRepository repo)
        {
            _repo = repo;
        }

        public Task<GameHistory?> Handle(GetGameByMatchIdQuery request, CancellationToken cancellationToken)
        {
            return _repo.GetByMatchIdAsync(request.MatchId);
        }
    }
}
