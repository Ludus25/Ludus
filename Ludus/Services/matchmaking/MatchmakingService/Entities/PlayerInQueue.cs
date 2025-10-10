namespace MatchmakingService.Entities;

public class PlayerInQueue
{
    public string PlayerId { get; private set; }
    public int Rating { get; private set; }

    public PlayerInQueue(string playerId, int rating)
    {
        PlayerId = playerId;
        Rating = rating;
    }
}
