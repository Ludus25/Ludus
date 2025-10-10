using Grpc.Net.Client;
using System.Threading.Tasks;
using User; // generated namespace from proto (adjust if different)

namespace MatchmakingService.Infrastructure.Grpc
{
    public class UserGrpcClient
    {
        private readonly UserService.UserServiceClient _client;
        public UserGrpcClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new UserService.UserServiceClient(channel);
        }

        public async Task<int?> GetRatingAsync(string playerId)
        {
            try
            {
                var resp = await _client.GetPlayerAsync(new GetPlayerRequest { PlayerId = playerId });
                return resp?.Player?.Rating;
            }
            catch
            {
                return null;
            }
        }
    }
}
