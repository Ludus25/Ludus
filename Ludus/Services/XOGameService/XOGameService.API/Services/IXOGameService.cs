using XOGameService.API.Models;

namespace XOGameService.API.Services
{
    public interface IXOGameService
    {
        Task<GameState> CreateGame(string playerXId, string playerOId);
        Task<GameState?> GetGame(string gameId);
        Task<GameState> MakeMove(string gameId, int CellIndex, int expectedVersion, string actingUserId);
        Task<GameState?> GetActiveGameByUserId(string userId);

    }
}
