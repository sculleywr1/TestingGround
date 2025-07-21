using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class ChatService
    {
        private readonly ApplicationDbContext _db;

        public ChatService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ChatMessage> AddMessageAsync(string senderId, string? receiverId, string message)
        {
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };
            _db.Messages.Add(chatMessage);
            await _db.SaveChangesAsync();
            return chatMessage;
        }
    }
}
