using Grpc.Core;
using Game;
using XOGameService.API.Services;

namespace XOGameService.API.Services
{
    public class GameGrpcService : Game.GameService.GameServiceBase
    {
        private readonly IXOGameService _xoGameService;
        private readonly ILogger<GameGrpcService> _logger;
        private readonly IConfiguration _config;

        public GameGrpcService(
            IXOGameService xoGameService,
            ILogger<GameGrpcService> logger,
            IConfiguration config)
        {
            _xoGameService = xoGameService;
            _logger = logger;
            _config = config;
        }

        public override async Task<StartGameResponse> StartGame(
            StartGameRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation(
                "[gRPC-SERVER] Primljen zahtev - MatchId: {MatchId}, Igrači: {Count}",
                request.MatchId,
                request.Players.Count);

            // Validacija
            if (request.Players.Count != 2)
            {
                _logger.LogWarning("[gRPC-SERVER] Pogrešan broj igrača: {Count}", request.Players.Count);
                return new StartGameResponse
                {
                    Success = false,
                    GameServerUrl = "",
                    Message = "Potrebna su tačno 2 igrača"
                };
            }

            try
            {
                // Poziva postojeći CreateGame metod
                var player1Id = request.Players[0].PlayerId;
                var player2Id = request.Players[1].PlayerId;
                
                _logger.LogInformation(
                    "[gRPC-SERVER] Creating game for players: {Player1} vs {Player2}",
                    player1Id,
                    player2Id);

                var gameState = await _xoGameService.CreateGame(player1Id, player2Id);

                // ✅ Čita frontend URL iz konfiguracije
                var frontendUrl = _config["GameFrontendUrl"] ?? "http://localhost:5173";
                var gameUrl = $"{frontendUrl}/game/{gameState.GameId}?player1={player1Id}&player2={player2Id}";
                
                _logger.LogInformation(
                    "[gRPC-SERVER] Igra kreirana - GameId: {GameId}, URL: {Url}",
                    gameState.GameId,
                    gameUrl);

                return new StartGameResponse
                {
                    Success = true,
                    GameServerUrl = gameUrl,
                    Message = "Igra uspešno kreirana"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[gRPC-SERVER] Greška pri kreiranju igre");
                
                return new StartGameResponse
                {
                    Success = false,
                    GameServerUrl = "",
                    Message = $"Greška: {ex.Message}"
                };
            }
        }
    }
}