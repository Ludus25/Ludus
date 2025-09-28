namespace MatchmakingService.Application.DTOs
{
    public class JoinRequest
    {
        public required string PlayerId { get; set; }
        public int Rating { get; set; }
    }
}
