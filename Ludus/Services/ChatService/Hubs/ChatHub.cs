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
            var gameId = Context.GetHttpContext()?.Request.Query["gameId"].ToString();

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(gameId))
            {
                _userService.AddUser(Context.ConnectionId, user, gameId);

                await Clients.Group(gameId).SendAsync("UpdateUserList", _userService.GetOnlineUsers(gameId));

                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var gameId = _userService.GetGameId(Context.ConnectionId);
            _userService.RemoveUser(Context.ConnectionId);

            if (!string.IsNullOrEmpty(gameId))
            {
                await Clients.Group(gameId).SendAsync("UpdateUserList", _userService.GetOnlineUsers(gameId));
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToGame(string gameId, string user, string message)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(gameId))
                return;

            var msg = new Message
            {
                Sender = user,
                Content = message,
                SentAt = DateTime.UtcNow,
                GameId = gameId
            };

            await _messageService.AddMessageAsync(msg);

            await Clients.Group(gameId).SendAsync("ReceiveMessage", msg);
        }

        public async Task LoadOlderMessages(string gameId, int count, DateTime? before = null)
        {
            List<Message> olderMessages;

            if (before == null || before == default(DateTime))
            {
                olderMessages = await _messageService.GetRecentMessagesAsync(gameId, count);
            }
            else
            {
                olderMessages = await _messageService.GetOlderMessagesAsync(gameId, count, before.Value);
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
