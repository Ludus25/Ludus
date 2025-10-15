using System.Collections.Generic;

namespace ChatService.Services
{
    public interface IUserService
    {
        void AddUser(string connectionId, string username, string gameId);
        void RemoveUser(string connectionId);
        List<string> GetOnlineUsers(string gameId);
        string? GetGameId(string connectionId);
        (string Username, string GameId)? GetUserByConnectionId(string connectionId);
    }
}
