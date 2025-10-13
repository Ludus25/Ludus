using Common.Entities;
using Interfaces;
using MediatR;

namespace Queries
{
    public class GetGamesByUserQueryHandler : IRequestHandler<GetGamesByUserQuery, IEnumerable<GameHistory>>
    {
        private readonly IGameHistoryRepository _repo;

        public GetGamesByUserQueryHandler(IGameHistoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<GameHistory>> Handle(GetGamesByUserQuery request, CancellationToken cancellationToken)
        {
            return _repo.GetGamesByUserAsync(request.UserId, request.Limit, request.Offset);
        }
    }
}
