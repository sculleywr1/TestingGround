using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string message, string? receiverId)
        {
            var senderId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (senderId == null)
                return;

            var chatMessage = await _chatService.AddMessageAsync(senderId,
                string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                message);

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
