using Microsoft.AspNetCore.Mvc;
using ChatService.Entities;
using ChatService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                Content = "Chat service is running",
                SentAt = DateTime.UtcNow
            });
        }

        // GET api/chat/messages/{gameId}
        [HttpGet("messages/{gameId}")]
        public async Task<ActionResult<List<Message>>> GetMessages(string gameId)
        {
            var messages = await _messageService.GetMessagesAsync(gameId);
            return Ok(messages);
        }
    }
}
