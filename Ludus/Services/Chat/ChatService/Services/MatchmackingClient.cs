public interface IMatchmakingClient
{
    Task<string?> GetMatchIdForPlayerAsync(string playerId);
}

public class MatchStatusResponse
{
    public string Status { get; set; } = null!;
    public string MatchId { get; set; } = null!;
    public List<string> Players { get; set; } = new List<string>();
}

public class MatchmakingClient : IMatchmakingClient
{
    private readonly HttpClient _httpClient;

    public MatchmakingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetMatchIdForPlayerAsync(string playerId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MatchStatusResponse>($"api/matchmaking/status/{playerId}");

            if (response == null)
                return null;

            // Ako status nije "matched", nema gameId
            if (response.Status != "matched")
                return null;

            return response.MatchId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MatchmakingClient] Error fetching matchId for {playerId}: {ex.Message}");
            return null;
        }
    }


    private class StatusResponse
    {
        public string Status { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
    }
}