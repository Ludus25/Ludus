using ChatService.Entities;
using System.Collections.Generic;

namespace ChatService.Services
{
    public interface IMessageService
    {
        void AddMessage(Message message);
        List<Message> GetMessages();
    }
}
