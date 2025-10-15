using Common.Entities;
using MediatR;

namespace Queries
{
    public record GetGameByMatchIdQuery(string MatchId) : IRequest<GameHistory?>;
}
