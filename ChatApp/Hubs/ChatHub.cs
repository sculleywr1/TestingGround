using ChatApp.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SendMessage(string message, string? receiverId)
        {
            var senderId = Context.UserIdentifier;
            if (senderId == null)
                return;

            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                Message = message
            };

            _db.Messages.Add(chatMessage);
            await _db.SaveChangesAsync();

            if (chatMessage.ReceiverId == null)
            {
                await Clients.All.SendAsync("ReceiveMessage", chatMessage.Id, senderId, message, chatMessage.Timestamp);
            }
            else
            {
                await Clients.User(chatMessage.ReceiverId).SendAsync("ReceivePrivateMessage", chatMessage.Id, senderId, message, chatMessage.Timestamp);
                await Clients.Caller.SendAsync("ReceivePrivateMessage", chatMessage.Id, senderId, message, chatMessage.Timestamp);
            }
        }
    }
}
