using Common.Entities;
using MediatR;

namespace Queries
{
    public record GetGamesByEmailQuery(string Email, int Limit = 50, int Offset = 0) : IRequest<IEnumerable<GameHistory>>;
}