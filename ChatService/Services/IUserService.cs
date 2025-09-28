using System.Collections.Generic;

namespace ChatService.Services
{
    public interface IUserService
    {
        void AddUser(string connectionId, string username);
        void RemoveUser(string connectionId);
        List<string> GetOnlineUsers();
    }
}
