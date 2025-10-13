using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ChatService.Services
{
    public class UserService : IUserService
    {
        private class UserInfo
        {
            public string Username { get; set; } = string.Empty;
            public string GameId { get; set; } = string.Empty;
        }

        private readonly ConcurrentDictionary<string, UserInfo> _onlineUsers = new();

        public void AddUser(string connectionId, string username, string gameId)
        {
            _onlineUsers[connectionId] = new UserInfo { Username = username, GameId = gameId };
        }

        public void RemoveUser(string connectionId)
        {
            _onlineUsers.TryRemove(connectionId, out _);
        }

        public List<string> GetOnlineUsers(string gameId)
        {
            return _onlineUsers.Values
                .Where(u => u.GameId == gameId)
                .Select(u => u.Username)
                .ToList();
        }

        public string? GetGameId(string connectionId)
        {
            return _onlineUsers.TryGetValue(connectionId, out var userInfo) ? userInfo.GameId : null;
        }

        public (string Username, string GameId)? GetUserByConnectionId(string connectionId)
        {
            if (_onlineUsers.TryGetValue(connectionId, out var userInfo))
            {
                return (userInfo.Username, userInfo.GameId);
            }
            return null;
        }
    }

}
