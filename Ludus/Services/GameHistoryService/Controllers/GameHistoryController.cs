using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Queries;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GameHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGamesByPlayer(string userId, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            var games = await _mediator.Send(new GetGamesByUserQuery(userId, limit, offset));
            var result = games.Select(g => new {
                g.MatchId,
                g.PlayerUserIds,
                g.StartedAt,
                g.EndedAt,
                g.MoveHistory,
                g.WinnerUserId,
                ChatCount = g.ChatMessages?.Count ?? 0
            });

            return Ok(result);
        }

        [HttpGet("match/{matchId}")]
        public async Task<IActionResult> GetByMatch(string matchId)
        {
            var match = await _mediator.Send(new GetGameByMatchIdQuery(matchId));
            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }

    }
}
