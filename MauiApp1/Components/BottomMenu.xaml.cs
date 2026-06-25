using MauiApp1.Models;
using MauiApp1.Services;
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

    private async void HomeButton_Tapped(object sender, TappedEventArgs e)
    {
        var stack = Shell.Current.Navigation.NavigationStack.ToArray();
        for (int i = stack.Length - 1; i > 0; i--)
        {
            Shell.Current.Navigation.RemovePage(stack[i]);
        }
        await Shell.Current.GoToAsync("//Home", false);
    }

    private async void MeditationsButton_Tapped(object sender, TappedEventArgs e)
    {
        var stack = Shell.Current.Navigation.NavigationStack.ToArray();
        for (int i = stack.Length - 1; i > 0; i--)
        {
            Shell.Current.Navigation.RemovePage(stack[i]);
        }
        await Shell.Current.GoToAsync("FullMeditation", false);
    }

    private async void SettingsButton_Tapped(object sender, TappedEventArgs e)
    {
        var stack = Shell.Current.Navigation.NavigationStack.ToArray();
        for (int i = stack.Length - 1; i > 0; i--)
        {
            Shell.Current.Navigation.RemovePage(stack[i]);
        }
        await Shell.Current.GoToAsync("Settings", false);
    }
    private async void RosaryButton_Tapped(object sender, TappedEventArgs e)
    {
       
        if (string.IsNullOrEmpty(_authService.Token)) return;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);

        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        int userRole = int.Parse(roleClaim?.Value ?? "5");
        if (userRole == 0 || userRole == 1)
        {
            await Shell.Current.GoToAsync("MyRosariesList");
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
                    await Shell.Current.GoToAsync("MyRosaryGroup", navigationParameter);
                }
                else
                {
                    await Shell.Current.GoToAsync("SelectParish", new Dictionary<string, object> { { "UserId", Id } });
                }
            }
        }
    }
}