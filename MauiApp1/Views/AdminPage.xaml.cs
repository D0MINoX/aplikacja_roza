using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class AdminPage : ContentPage
{
    private readonly AuthService _authService;

    public AdminPage(AuthService authService)
    {
        _authService   = authService;
        InitializeComponent();
    }

    private async void RosaryMenagement_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("AdminRosaries");
    }
    private async void RosaryAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RosaryAdd");
    }
    private async void MeditationAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("MeditationAdd");
    }

    private async void ChangePrivilagies_Tapped(object sender, TappedEventArgs e)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);
        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        string userRole = roleClaim?.Value ?? "4";
        int Role = int.Parse(userRole);
        var navigationParameter = new Dictionary<string, object>
        {
            { "UserRole", Role }
        };
        await Shell.Current.GoToAsync("ChangeUserPrivilagies",navigationParameter);
    }
}