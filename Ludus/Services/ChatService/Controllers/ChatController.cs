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
                Content = "Chat service is running", // promenjeno sa Text
                SentAt = DateTime.UtcNow             // promenjeno sa Timestamp
            });
        }

        [HttpGet("messages")]
        public async Task<ActionResult<List<Message>>> GetMessages()
        {
            var messages = await _messageService.GetMessagesAsync();
            return Ok(messages);
        }
    }
}