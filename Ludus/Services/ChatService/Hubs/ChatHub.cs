using Microsoft.AspNetCore.SignalR;
using ChatService.Entities;
using ChatService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatHub(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.GetHttpContext()?.Request.Query["user"].ToString();

            if (!string.IsNullOrEmpty(user))
            {
                _userService.AddUser(Context.ConnectionId, user);

                await Clients.All.SendAsync("UpdateUserList", _userService.GetOnlineUsers());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userService.RemoveUser(Context.ConnectionId);

            await Clients.All.SendAsync("UpdateUserList", _userService.GetOnlineUsers());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"SendMessage called by {user}: {message}");
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(user))
                return;

            var msg = new Message
            {
                Sender = user,
                Content = message,      // promenjeno
                SentAt = DateTime.UtcNow // promenjeno
            };

            await _messageService.AddMessageAsync(msg);

            await Clients.All.SendAsync("ReceiveMessage", msg);
        }


        public async Task LoadOlderMessages(int count, DateTime? before = null)
        {
            Console.WriteLine($"LoadOlderMessages called with count={count}, before={before}");

            List<Message> olderMessages;

            if (before == null || before == default(DateTime))
            {
                olderMessages = await _messageService.GetRecentMessagesAsync(count);
            }
            else
            {
                olderMessages = await _messageService.GetOlderMessagesAsync(count, before.Value);
            }

            if (olderMessages.Count == 0)
            {
                await Clients.Caller.SendAsync("NoMoreMessages");
            }
            else
            {
                await Clients.Caller.SendAsync("LoadOlderMessages", olderMessages);
            }
        }
    }
}
