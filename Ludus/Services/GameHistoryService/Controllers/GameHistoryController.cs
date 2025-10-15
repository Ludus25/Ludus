using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Queries;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var result = games.Select(g => new
            {
                g.MatchId,
                g.PlayerUserIds,
                g.PlayerEmails,
                g.StartedAt,
                g.EndedAt,
                g.MoveHistory,
                g.WinnerUserId
            });

            return Ok(result);
        }

        [HttpGet("email/{userEmail}")]
        public async Task<IActionResult> GetGamesByEmail(string userEmail, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            var games = await _mediator.Send(new GetGamesByEmailQuery(userEmail, limit, offset));
            var result = games.Select(g => new
            {
                g.MatchId,
                g.PlayerUserIds,
                g.PlayerEmails,
                g.StartedAt,
                g.EndedAt,
                g.MoveHistory,
                g.WinnerUserId
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
