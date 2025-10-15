using Common.Entities;

namespace Interfaces
{
    public interface IGameHistoryRepository
    {
        Task SaveGameAsync(GameHistory gameHistory, CancellationToken ct);
        Task AppendChatMessageAsync(string matchId, IEnumerable<ChatMessage> messages, CancellationToken ct);
        Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit, int offset, CancellationToken ct);
        Task<GameHistory?> GetByMatchIdAsync(string matchId, CancellationToken ct);
    }
}
