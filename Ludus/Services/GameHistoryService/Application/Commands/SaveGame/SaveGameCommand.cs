using Common.Entities;
using MediatR;

namespace Commands
{
    public record SaveGameCommand(GameHistory History) : IRequest;
}
