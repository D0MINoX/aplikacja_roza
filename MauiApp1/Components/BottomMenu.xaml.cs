using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1;

public partial class BottomMenu : ContentView
{
    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    public BottomMenu()
    {
       
        InitializeComponent();
        _authService = IPlatformApplication.Current.Services.GetRequiredService<AuthService>();
        _rosaryService = IPlatformApplication.Current.Services.GetRequiredService<RosaryService>();
    }

    private static async Task NavigateOnceAsync(string route, Type pageType, IDictionary<string, object>? parameters = null, bool animate = true, bool clearStack = false)
    {
        if (Shell.Current.CurrentPage?.GetType() == pageType) return;

        if (clearStack)
        {
            var stack = Shell.Current.Navigation.NavigationStack.ToArray();
            for (int i = stack.Length - 1; i > 0; i--)
            {
                Shell.Current.Navigation.RemovePage(stack[i]);
            }
        }

        if (parameters == null)
            await Shell.Current.GoToAsync(route, animate);
        else
            await Shell.Current.GoToAsync(route, animate, parameters);
    }

    private async void HomeButton_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//Home", false);
    }


    private async void SettingsButton_Tapped(object sender, TappedEventArgs e)
    {
        await NavigateOnceAsync("Settings", typeof(SettingsPage), animate: false, clearStack: true);
    }
    private async void RosaryButton_Tapped(object sender, TappedEventArgs e)
    {
       
        if (string.IsNullOrEmpty(_authService.Token))
        {
            await NavigateOnceAsync("LoginRequired", typeof(LoginRequiredPage));
            return;
        }
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);

        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        int userRole = int.Parse(roleClaim?.Value ?? "5");
        if (userRole == 0 || userRole == 1)
        {
            await NavigateOnceAsync("MyRosariesList", typeof(MyRosariesListPage));
        }
        else
        {

            var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
            if (IdClaim != null && int.TryParse(IdClaim.Value, out int Id))
            {
                int rosaryId;
                List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(Id);
                if (rosaryInfos != null && rosaryInfos.Count == 1)
                {
                    rosaryId = rosaryInfos[0].Id;
                    var navigationParameter = new Dictionary<string, object> { { "RosaryId", rosaryId.ToString() }, { "UserRole", userRole } };
                    await NavigateOnceAsync("MyRosaryGroup", typeof(MyRosaryPage), navigationParameter);
                }
                else
                {
                    await NavigateOnceAsync("SelectParish", typeof(SelectParish), new Dictionary<string, object> { { "UserId", Id } });
                }
            }
        }
    }
    private async void FullRosaryButton_Tapped(object sender, TappedEventArgs e) {
        await NavigateOnceAsync("FullRosary", typeof(FullRosaryPage), animate: false, clearStack: true);
    }
}