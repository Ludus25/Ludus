using ChatService.Entities;
using System.Collections.Generic;

namespace ChatService.Services
{
    public class MessageService : IMessageService
    {
        private readonly List<Message> _messages = new();

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }

        public List<Message> GetMessages()
        {
            return _messages;
        }
    }
}
