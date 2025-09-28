using Microsoft.AspNetCore.Mvc;
using ChatService.Entities;
using ChatService.Services;
using System.Collections.Generic;

namespace ChatService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ChatController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("test")]
        public ActionResult<Message> GetTestMessage()
        {
            return Ok(new Message
            {
                Sender = "System",
                Text = "Chat service is running",
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("messages")]
        public ActionResult<List<Message>> GetMessages()
        {
            var messages = _messageService.GetMessages();
            return Ok(messages);
        }
    }
}
