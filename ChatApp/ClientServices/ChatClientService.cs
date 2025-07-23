using ChatApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace ChatApp.ClientServices
{
    public class ChatClientService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigation;

        public HubConnection? Connection { get; private set; }
        public string Token { get; private set; } = string.Empty;

        public event Action<ChatMessageView>? MessageReceived;

        public ChatClientService(IHttpClientFactory factory, NavigationManager navigation)
        {
            _navigation = navigation;
            _http = factory.CreateClient();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync(_navigation.ToAbsoluteUri("auth/login"), new LoginRequest { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                Token = result?.Token ?? string.Empty;
                await StartConnectionAsync();
                return true;
            }
            return false;
        }

        public async Task RegisterAsync(string email, string password)
        {
            await _http.PostAsJsonAsync(_navigation.ToAbsoluteUri("auth/register"), new LoginRequest { Email = email, Password = password });
            await LoginAsync(email, password);
        }

        private async Task StartConnectionAsync()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(_navigation.ToAbsoluteUri("/chathub"), options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(Token);
                })
                .Build();

            Connection.On<int, string, string, DateTime>("ReceiveMessage", (id, sender, text, ts) =>
            {
                MessageReceived?.Invoke(new ChatMessageView { Id = id, SenderId = sender, Text = text, Timestamp = ts, IsPrivate = false });
            });

            Connection.On<int, string, string, DateTime>("ReceivePrivateMessage", (id, sender, text, ts) =>
            {
                MessageReceived?.Invoke(new ChatMessageView { Id = id, SenderId = sender, Text = text, Timestamp = ts, IsPrivate = true });
            });

            await Connection.StartAsync();
        }

        public async Task SendMessageAsync(string message, string? receiver)
        {
            if (Connection == null || string.IsNullOrWhiteSpace(message)) return;

            await Connection.SendAsync("SendMessage", message, string.IsNullOrWhiteSpace(receiver) ? null : receiver);
        }
    }

    public class ChatMessageView
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsPrivate { get; set; }
    }
}
