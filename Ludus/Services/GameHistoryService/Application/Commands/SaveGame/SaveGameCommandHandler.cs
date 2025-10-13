using Interfaces;
using MediatR;

namespace Commands
{
    public class SaveGameCommandHandler : IRequestHandler<SaveGameCommand>
    {
        private readonly IGameHistoryRepository _repo;

        public SaveGameCommandHandler(IGameHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SaveGameCommand request, CancellationToken cancellationToken)
        {
            await _repo.SaveGameAsync(request.History);
        }
    }
}
