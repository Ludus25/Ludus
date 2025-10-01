public interface IGameHistoryService
{
    Task<IEnumerable<GameHistory>> GetHistoryByUserAsync(string userId);
    Task<GameHistory> GetHistoryByMatchIdAsync(string matchId);
}
