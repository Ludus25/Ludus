[ApiController]
[Route("api/history")]
public class HistoryController : ControllerBase
{
    private readonly IGameHistoryService _historyService;

    public HistoryController(IGameHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var history = await _historyService.GetHistoryByUserAsync(userId);
        return Ok(history);
    }

    [HttpGet("match/{matchId}")]
    public async Task<IActionResult> GetByMatch(string matchId)
    {
        var match = await _historyService.GetHistoryByMatchIdAsync(matchId);
        return Ok(match);
    }
}
