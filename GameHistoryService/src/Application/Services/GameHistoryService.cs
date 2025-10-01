public class GameHistoryService : IGameHistoryService
{
    private readonly IGameHistoryRepository _repository;

    public GameHistoryService(IGameHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GameHistory>> GetHistoryByUserAsync(string userId)
    {
        return await _repository.GetByUserAsync(userId);
    }

    public async Task<GameHistory> GetHistoryByMatchIdAsync(string matchId)
    {
        return await _repository.GetByMatchIdAsync(matchId);
    }
}
