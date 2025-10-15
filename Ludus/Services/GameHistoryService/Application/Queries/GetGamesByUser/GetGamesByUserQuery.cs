using Common.Entities;
using MediatR;

namespace Queries
{
    public record GetGamesByUserQuery(string UserId, int Limit = 50, int Offset = 0) : IRequest<IEnumerable<GameHistory>>;
}
