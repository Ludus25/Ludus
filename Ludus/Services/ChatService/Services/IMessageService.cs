using ChatService.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public interface IMessageService
    {
        Task AddMessageAsync(Message message);
        Task<List<Message>> GetMessagesAsync(string gameId, int limit = 50);
        Task<List<Message>> GetRecentMessagesAsync(string gameId, int count);
        Task<List<Message>> GetOlderMessagesAsync(string gameId, int count, DateTime before);
    }
}
