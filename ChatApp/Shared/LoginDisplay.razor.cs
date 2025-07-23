using ChatApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Shared
{
    public partial class LoginDisplay : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Inject]
        public SignInManager<ApplicationUser> SignInManager { get; set; } = default!;

        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; } = default!;

        private async Task SignOut()
        {
            await SignInManager.SignOutAsync();
            Navigation.NavigateTo("/");
        }
    }
}
