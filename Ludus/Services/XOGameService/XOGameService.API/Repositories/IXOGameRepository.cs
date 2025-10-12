using XOGameService.API.Models;
using XOGameService.API.Models.Enums;

namespace XOGameService.API.Repositories
{
    public interface IXOGameRepository
    {
        Task<GameState?> GetAsync(string gameId);
        Task CreateAsync(GameState gameState);
        Task<bool> TryUpdateAsync(GameState newState, int expectedVersion);
        Task<GameState?> GetGameByUserIdAndStatus(string userId, GameStatus gameStatus);
    }
}
