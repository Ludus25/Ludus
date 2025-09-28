using Microsoft.AspNetCore.Mvc;
using MatchmakingService.Application.Commands;
using MatchmakingService.Application.DTOs;
using MatchmakingService.Application.Services;
using System.Linq;

namespace MatchmakingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchmakingController : ControllerBase
{
    private readonly JoinCommandHandler _commandHandler;
    private readonly IMatchRepository _repo;

    public MatchmakingController(JoinCommandHandler commandHandler, IMatchRepository repo)
    {
        _commandHandler = commandHandler;
        _repo = repo;
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] JoinRequest payload)
    {
        var cmd = new JoinCommand(payload.PlayerId, payload.Rating);
        var res = await _commandHandler.HandleAsync(cmd);
        return Ok(new { message = res });
    }

    [HttpGet("status/{playerId}")]
    public IActionResult Status(string playerId)
    {
        var match = _repo.GetMatchForPlayer(playerId);
        if (match != null)
            return Ok(new { status = "matched", matchId = match.MatchId, players = match.Players.Select(p => p.PlayerId).ToArray() });

        if (_repo.IsPlayerInQueue(playerId))
            return Ok(new { status = "searching" });

        return Ok(new { status = "not_found" });
    }

    [HttpGet("queue")]
    public IActionResult Queue()
    {
        var snapshot = _repo.GetQueueSnapshot();
        return Ok(snapshot.Select(p => new { p.PlayerId, p.Rating }));
    }

    [HttpGet("matches")]
    public IActionResult Matches()
    {
        var matches = _repo.GetMatchesSnapshot();
        return Ok(matches.Select(m => new { m.MatchId, players = m.Players.Select(p => new { p.PlayerId, p.Rating }) }));
    }
}
