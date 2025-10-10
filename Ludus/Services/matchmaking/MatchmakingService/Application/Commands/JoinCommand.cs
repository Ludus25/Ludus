namespace MatchmakingService.Application.Commands
{
    public class JoinCommand
    {
        public string PlayerId { get; }
        public int Rating { get; }

        public JoinCommand(string playerId, int rating)
        {
            PlayerId = playerId;
            Rating = rating;
        }
    }
}
