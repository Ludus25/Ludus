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

        public ChatHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.GetHttpContext()?.Request.Query["user"].ToString();
            var gameId = Context.GetHttpContext()?.Request.Query["gameId"].ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(gameId))
            {
                await Clients.Caller.SendAsync("NoMatchFound");
                await base.OnConnectedAsync();
                return;
            }

            _userService.AddUser(Context.ConnectionId, user, gameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("UpdateUserList", _userService.GetOnlineUsers(gameId));

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

        public async Task SendMessageToGame(string message)
        {
            var userTuple = _userService.GetUserByConnectionId(Context.ConnectionId);
            if (userTuple == null) return;

            var (username, gameId) = userTuple.Value;

            var msg = new Message
            {
                Sender = username,
                Content = message,
                SentAt = DateTime.UtcNow,
                GameId = gameId
            };


            await Clients.Group(gameId).SendAsync("ReceiveMessage", msg);
        }

        public async Task LoadOlderMessages(string gameId, int count, DateTime? before = null)
        {
            List<Message> olderMessages = new List<Message>();

            await Clients.Caller.SendAsync("LoadOlderMessages", olderMessages);
        }
    }
}