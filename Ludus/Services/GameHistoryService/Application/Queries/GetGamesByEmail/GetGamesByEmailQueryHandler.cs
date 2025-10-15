using Common.Entities;
using Interfaces;
using MediatR;

namespace Queries
{
    public class GetGamesByEmailQueryHandler : IRequestHandler<GetGamesByEmailQuery, IEnumerable<GameHistory>>
    {
        private readonly IGameHistoryRepository _repo;

        public GetGamesByEmailQueryHandler(IGameHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<GameHistory>> Handle(GetGamesByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetGamesByEmailAsync(request.Email, request.Limit, request.Offset, cancellationToken);
        }
    }
}
