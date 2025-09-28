using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ChatService.Services
{
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<string, string> _onlineUsers = new();

        public void AddUser(string connectionId, string username)
        {
            _onlineUsers[connectionId] = username;
        }

        public void RemoveUser(string connectionId)
        {
            _onlineUsers.TryRemove(connectionId, out _);
        }

        public List<string> GetOnlineUsers()
        {
            return new List<string>(_onlineUsers.Values);
        }
    }
}
