using Entities;

namespace Interfaces
{
    public interface IGameHistoryService
    {
        Task SaveGameAsync(GameHistory history);
        Task AppendChatAsync(string matchId, IEnumerable<ChatMessage> messages);
        Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit = 50, int offset = 0);
        Task<GameHistory?> GetByMatchIdAsync(string matchId);
    }
}
