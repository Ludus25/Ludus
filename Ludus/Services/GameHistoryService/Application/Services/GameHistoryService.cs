using Interfaces;
using Common.Entities;

namespace Services
{
    public class GameHistoryService : IGameHistoryService
    {
        private readonly IGameHistoryRepository _repository;

        public GameHistoryService(IGameHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task AppendChatAsync(string matchId, IEnumerable<ChatMessage> messages)
        {
            return _repository.AppendChatMessageAsync(matchId, messages);
        }

        public Task<GameHistory?> GetByMatchIdAsync(string matchId)
        {
            return _repository.GetByMatchIdAsync(matchId);
        }

        public Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit = 50, int offset = 0)
        {
            return _repository.GetGamesByUserAsync(userId, limit, offset);
        }

        public Task SaveGameAsync(GameHistory gameHistory)
        {
            return _repository.SaveGameAsync(gameHistory);
        }
    }
}
