using ChatApp.ClientServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private ChatClientService ChatClient { get; set; } = default!;

        private List<ChatMessageView> Messages { get; set; } = new();
        private string Message { get; set; } = string.Empty;
        private string Receiver { get; set; } = string.Empty;
        private string Email { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;

        private bool IsConnected => ChatClient.Connection?.State == HubConnectionState.Connected;

        protected override void OnInitialized()
        {
            ChatClient.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(ChatMessageView msg)
        {
            Messages.Add(msg);
            InvokeAsync(StateHasChanged);
        }

        private async Task Login()
        {
            await ChatClient.LoginAsync(Email, Password);
        }

        private async Task Register()
        {
            await ChatClient.RegisterAsync(Email, Password);
        }

        private async Task Send()
        {
            await ChatClient.SendMessageAsync(Message, Receiver);
            Message = string.Empty;
            Receiver = string.Empty;
        }

        private async Task HandleKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Send();
            }
        }
    }
}
