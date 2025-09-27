using Microsoft.AspNetCore.Mvc;
using XOGameService.API.Dtos;
using XOGameService.API.Models;
using XOGameService.API.Services;

namespace XOGameService.API.Controllers
{
    [ApiController]
    [Route("api/v1/xo-game")]
    public class XOGameController : ControllerBase
    {
        private readonly IXOGameService _gameService;

        public XOGameController(IXOGameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        private string ActingUserId() =>
            Request.Headers.TryGetValue("X-UserId", out var v) ? v.ToString() : "anonymous";

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
            var gameState = await _gameService.MakeMove(id, makeMoveDto.CellIndex, makeMoveDto.Version, ActingUserId());
            return Ok(gameState);
        }
    }
}
