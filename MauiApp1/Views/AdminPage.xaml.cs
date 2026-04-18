using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class AdminPage : ContentPage
{
    private int UserRole { get; set; }
    private int UserId { get; set; }
    private readonly AuthService _authService;

    public AdminPage(AuthService authService)
    {
        _authService   = authService;
        InitializeComponent();
       
    }
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
  
        DecodeToken();
        if (UserRole == 1)
        {
            ParishAdd.IsVisible = false;
            Agrements.IsVisible = false;
            MeditationsAdd.IsVisible = false;
            ErrorReports.IsVisible = false;
        }
        if(UserRole == 2){
            RosaryAdd.IsVisible = false;
            ParishAdd.IsVisible = false;
            Agrements.IsVisible = false;
            MeditationsAdd.IsVisible = false;
            ErrorReports.IsVisible = false;
        }
       
    }
    private async void DecodeToken() {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);
        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        string userRole = roleClaim?.Value ?? "4";
        UserRole = int.Parse(userRole);
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        int.TryParse(IdClaim.Value, out int Id);
        UserId = Id;
        
    }

    private async void RosaryMenagement_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("AdminRosaries");
    }
    private async void RosaryAdd_Tapped(object sender, TappedEventArgs e)
    {
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "UserRole", UserRole },
            {"UserId",UserId }
        };
        await Shell.Current.GoToAsync("RosaryAdd",navigationParameter);
    }
    private async void MeditationAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("MeditationAdd");
    }
    private async void ParishAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ParishAdd");
    }

    private async void ChangePrivilagies_Tapped(object sender, TappedEventArgs e)
    {
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "UserRole", UserRole }
        };
        await Shell.Current.GoToAsync("ChangeUserPrivilagies",navigationParameter);
    }
    private async void ExternalNumbers_Tapped(object sender, TappedEventArgs e) {
        await Shell.Current.GoToAsync("ExternalNumbers");
    }
    private async void Agrements_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("AgreementsMenagement");
    }
    private async void ErrorReports_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ErrorManagement");
    }
}