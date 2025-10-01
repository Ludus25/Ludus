using Entities;

namespace Interfaces
{
    public interface IGameHistoryRepository
    {
        Task SaveGameAsync(GameHistory gameHistory);
        Task AppendChatMessageAsync(string matchId, IEnumerable<ChatMessage> messages);
        Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit = 50, int offset = 0);
        Task<GameHistory?> GetByMatchIdAsync(string matchId);
    }
}
