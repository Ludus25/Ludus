using Microsoft.AspNetCore.SignalR;
using ChatService.Entities;
using ChatService.Services;

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
            var msg = new Message
            {
                Sender = user,
                Text = message,
                Timestamp = DateTime.UtcNow
            };

            _messageService.AddMessage(msg);

            await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}
