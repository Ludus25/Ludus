using ChatService.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public interface IMessageService
    {
        Task AddMessageAsync(Message message);
        Task<List<Message>> GetRecentMessagesAsync(int count);
        Task<List<Message>> GetOlderMessagesAsync(int count, DateTime before);
        Task<List<Message>> GetMessagesAsync(int limit = 50); // opcionalno, ali ako ga koristiš u kodu — mora ovde biti
    }
}
