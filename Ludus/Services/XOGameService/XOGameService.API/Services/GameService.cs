using MassTransit;
using Common.Dto;
using Microsoft.AspNetCore.SignalR;
using XOGameService.API.Exceptions;
using XOGameService.API.Exceptions.Enums;
using XOGameService.API.Hubs;
using XOGameService.API.Models;
using XOGameService.API.Models.Enums;
using XOGameService.API.Repositories;
using Common.Entities;

namespace XOGameService.API.Services
{
    public class GameService : IXOGameService
    {
        private readonly IXOGameRepository _repository;
        private readonly IHubContext<GameHub> _gameHubContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public GameService(IXOGameRepository repository, IHubContext<GameHub> gameHubContext, IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _gameHubContext = gameHubContext ?? throw new ArgumentNullException(nameof(gameHubContext));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        private static void SetEmailForUser(GameState game, string userId, string email)
        {
            if (userId == game.PlayerXId)
            {
                game.PlayerXEmail = email;
            }
            else
            {
                game.PlayerOEmail = email;
            }
        }

        public async Task<GameState> CreateGame(string playerXId, string playerOId)
        {
            var gameState = new GameState
            {
                PlayerXId = playerXId,
                PlayerOId = playerOId
            };

            await _repository.CreateAsync(gameState);
            return gameState;
        }

        public async Task<GameState?> GetGame(string gameId)
        {
            return await _repository.GetAsync(gameId);
        }

        public async Task<GameState> MakeMove(string gameId, int cellIndex, int expectedVersion, string actingUserId, string actingUserEmail)
        {
            if (cellIndex < 0 || cellIndex > 8)
            {
                throw new XOGameException(
                    GameErrorCode.InvalidCellIndex,
                    "Cell index out of range."
                    );
            }

            var currentState = await _repository.GetAsync(gameId);
            if (currentState == null)
            {
                throw new XOGameException(
                    GameErrorCode.GameNotFound,
                    $"Game {gameId} not found."
                    );
            }

            SetEmailForUser(currentState, actingUserId, actingUserEmail);

            if (currentState.Status != GameStatus.InProgress)
            {
                throw new XOGameException(
                    GameErrorCode.GameAlreadyFinished,
                    $"Game {gameId} already finished."
                    );
            }

            var expectedPlayer = currentState.NextPlayer;
            var actingIsX = actingUserId == currentState.PlayerXId;
            var actingIsO = actingUserId == currentState.PlayerOId;

            if (!(actingIsX || actingIsO))
            {
                throw new XOGameException(
                    GameErrorCode.NotParticipant,
                    "You are not participant of this game."
                    );
            }

            if (
                (expectedPlayer == 'X' && !actingIsX) ||
                (expectedPlayer == 'O' && !actingIsO)
                )
            {
                throw new XOGameException(
                    GameErrorCode.NotYourTurn,
                    "It is not your turn."
                    );
            }

            if (currentState.Cells[cellIndex] != Cell.Empty)
            {
                throw new XOGameException(
                    GameErrorCode.CellTaken, 
                    "Cell already taken."
                    );
            }

            var expectedPlayerEnum = expectedPlayer == 'X' ? Cell.X : Cell.O;
            currentState.Cells[cellIndex] = expectedPlayerEnum;
            currentState.Version++;
            currentState.UpdatedAt = DateTime.UtcNow;

            currentState.MoveHistory = string.IsNullOrEmpty(currentState.MoveHistory) ? cellIndex.ToString() : $"{currentState.MoveHistory},{cellIndex.ToString()}";

            bool isWinning = IsWinning(currentState.Cells, expectedPlayerEnum);
            bool isFull = IsFull(currentState.Cells);

            if (isWinning || isFull)
            {
                if (isWinning)
                {
                    currentState.Status = expectedPlayer == 'X' ? GameStatus.XWon : GameStatus.OWon;
                }
                else
                {
                    currentState.Status = GameStatus.Draw;
                }

                string? winnerUserId = currentState.Status == GameStatus.Draw ? null : currentState.Status == GameStatus.XWon ? currentState.PlayerXId : currentState.PlayerOId;
                await _publishEndpoint.Publish(new GameEndedEvent(
                    MatchId: currentState.GameId,
                    PlayerUserIds: new List<string> { currentState.PlayerXId, currentState.PlayerOId },
                    PlayerEmails: new List<string> { currentState.PlayerXEmail, currentState.PlayerOEmail},
                    StartedAt: currentState.CreatedAt,
                    EndedAt: DateTime.UtcNow,
                    MoveHistory: currentState.MoveHistory,
                    WinnerUserId: winnerUserId
                    ));
            }
            else
            {
                currentState.NextPlayer = expectedPlayer == 'X' ? 'O' : 'X';
            }

            var updated = await _repository.TryUpdateAsync(currentState, expectedVersion);
            if (!updated)
            {
                throw new XOGameException(
                    GameErrorCode.VersionConflict,
                    "Version conflict."
                    );
            }

            await _gameHubContext.Clients.Group(gameId).SendAsync("MoveMade", currentState);

            return currentState;
        }

        private static bool IsFull(Cell[] c) => Array.TrueForAll(c, x => x != Cell.Empty);

        private static bool IsWinning(Cell[] c, Cell p)
        {
            int[,] lines = {
                {0,1,2},{3,4,5},{6,7,8},
                {0,3,6},{1,4,7},{2,5,8},
                {0,4,8},{2,4,6}
            };

            for (int i = 0; i < 8; i++)
                if (c[lines[i, 0]] == p && c[lines[i, 1]] == p && c[lines[i, 2]] == p)
                    return true;

            return false;
        }

        public async Task<GameState?> GetActiveGameByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var game = await _repository.GetGameByUserIdAndStatus(userId, GameStatus.InProgress);
            return game;
        }
    }


}
