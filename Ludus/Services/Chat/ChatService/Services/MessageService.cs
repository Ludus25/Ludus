using ChatService.Data;
using ChatService.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Services
{
    public class MessageService : IMessageService
    {
        private readonly ChatDbContext _context;

        public MessageService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetMessagesAsync(string gameId, int limit = 50)
        {
            var messages = await _context.Messages
                .Where(m => m.GameId == gameId)
                .OrderByDescending(m => m.SentAt)
                .Take(limit)
                .ToListAsync();

            return messages.OrderBy(m => m.SentAt).ToList();
        }

        public async Task<List<Message>> GetRecentMessagesAsync(string gameId, int count)
        {
            var messages = await _context.Messages
                .Where(m => m.GameId == gameId)
                .OrderByDescending(m => m.SentAt)
                .Take(count)
                .ToListAsync();

            return messages.OrderBy(m => m.SentAt).ToList();
        }

        public async Task<List<Message>> GetOlderMessagesAsync(string gameId, int count, DateTime before)
        {
            var messages = await _context.Messages
                .Where(m => m.GameId == gameId && m.SentAt < before)
                .OrderByDescending(m => m.SentAt)
                .Take(count)
                .ToListAsync();

            return messages.OrderBy(m => m.SentAt).ToList();
        }
    }
}
