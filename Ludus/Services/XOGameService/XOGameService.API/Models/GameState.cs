using XOGameService.API.Models.Enums;

namespace XOGameService.API.Models
{
    public class GameState
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public string PlayerXId { get; set; } = string.Empty;
        public string PlayerOId { get; set; } = string.Empty;
        public string PlayerXEmail { get; set; } = string.Empty;
        public string PlayerOEmail { get; set; } = string.Empty;
        public Cell[] Cells { get; set; } = Enumerable.Repeat(Cell.Empty, 9).ToArray();
        public char NextPlayer { get; set; } = 'X';
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public int Version { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string MoveHistory { get; set; } = string.Empty;
    }
}
