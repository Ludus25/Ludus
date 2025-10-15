using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XOGameService.API.Dtos;
using XOGameService.API.Models;
using XOGameService.API.Models.Enums;
using XOGameService.API.Services;

namespace XOGameService.API.Controllers
{
    [ApiController]
    [Route("api/v1/xo-game")]
    public class XOGameController : ControllerBase
    {
        private readonly IXOGameService _gameService;
        private readonly ILogger<XOGameController> _logger;

        public XOGameController(IXOGameService gameService, ILogger<XOGameController> logger)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string ActingUserId()
        {
            if (Request.Headers.TryGetValue("X-UserId", out var value))
            {
                return value.ToString();
            }

            _logger.LogWarning("Missing X-UserId header in request from {RemoteIp}",
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            return "anonymous";
        }

        private string ActingUserEmail()
        {
            if (Request.Headers.TryGetValue("X-UserEmail", out var value))
            {
                return value.ToString();
            }

            var emailClaim = User?.Claims?.FirstOrDefault(c => c.Type.Contains("email", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrWhiteSpace(emailClaim))
            {
                return emailClaim;   
            }

            _logger.LogWarning("Missing X-UserEmail header in request from {RemoteIp}",
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            return "anonymousEmail";
        }

        [HttpPost]
        public async Task<ActionResult<GameState>> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            if (string.IsNullOrWhiteSpace(createGameDto.PlayerXId) || string.IsNullOrWhiteSpace(createGameDto.PlayerOId))
                return BadRequest("Both players must be provided.");

            var gameState = await _gameService.CreateGame(createGameDto.PlayerXId, createGameDto.PlayerOId);
            return CreatedAtAction(nameof(GetGame), new { id = gameState.GameId }, gameState);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameState>> GetGame(string id)
        {
            var gameState = await _gameService.GetGame(id);
            if (gameState is null) return NotFound();
            return gameState;
        }

        [HttpPost("{id}/move")]
        public async Task<ActionResult<GameState>> Move(string id, [FromBody] MakeMoveDto makeMoveDto)
        {
            var gameState = await _gameService.MakeMove(id, makeMoveDto.CellIndex, makeMoveDto.Version, ActingUserId(), ActingUserEmail());
            return Ok(gameState);
        }

        [HttpGet("active")]
        public async Task<ActionResult<GameState>> GetActiveGame()
        {
            var userId = ActingUserId();

            var headers = string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"));
            _logger.LogInformation("Incoming headers: {Headers}", headers);

            if (string.IsNullOrWhiteSpace(userId) || userId == "anonymous")
                return Unauthorized("Missing or invalid X-UserId header.");

            var game = await _gameService.GetActiveGameByUserId(userId);
            if (game is null)
                return NotFound("No active game found for this user.");

            return Ok(game);
        }
    }
}
