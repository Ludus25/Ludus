using XOGameService.API.Models;

namespace XOGameService.API.Repositories
{
    public interface IXOGameRepository
    {
        Task<GameState?> GetAsync(string gameId);
        Task CreateAsync(GameState gameState);
        Task<bool> TryUpdateAsync(GameState newState, int expectedVersion);
    }
}
