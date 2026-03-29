namespace MauiApp1;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls.Shapes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    private readonly ParishService _parishService;
    private int UserId { get; set; }
    public ProfilePage(AuthService authService,ParishService parishService, RosaryService rosaryService)
    {
        InitializeComponent();
        _authService = authService;
        _rosaryService = rosaryService;
        _parishService = parishService;
        DecodeToken(_authService.Token);

        //RosariesShow();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (UserId == 0) DecodeToken(_authService.Token);

        if (UserId > 0)
        {
            var response = await _parishService.GetUserParish(UserId);
            ParishLabel.Text = response.isSuccess ? response.Data.Name : response.ErrorMessage;

            RosariesShow();
        }
    }
    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool IsLoggedout = await _authService.Logout();
        if (IsLoggedout)
        {
            await DisplayAlertAsync("Informacja", "Konto zostało pomyślnie wylogowane", "OK");
        }
        else
        {
            await DisplayAlertAsync("BŁĄD", "Wylogowanie nie powiodło się, spróbuj ponownie", "OK");
        }
        await Shell.Current.GoToAsync("//Home");
    }
    public async Task DecodeToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);


        var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
        string userName = nameClaim?.Value ?? "Brak Imienia";

        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        string userRole = roleClaim?.Value ?? "Nie jesteś członkiem róży";
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        if (int.TryParse(IdClaim?.Value, out int id))
        {
            this.UserId = id; 
        }
        List<string> roles = ["Rola: admin", "Rola: główny zeletor", "Zelator: ", "Członek rózy: "];

        Name.Text = "Witaj " + userName;
        Role.Text = roles[int.Parse(userRole)];
        #if WINDOWS || MACCATALYST
        if (int.Parse(userRole) < 3)
        {
            adminBtn.IsVisible = true;
        }
        #endif
    }

    private async void RosariesShow()
    {
        List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(UserId);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (rosaryInfos == null || rosaryInfos.Count == 0)
            {
                Role.Text += "Nie należysz do żadnej róży";
                RosariesContainer.IsVisible = true;
            }
            else
            {
                Role.Text += rosaryInfos[0].Name;
            }
        });
    }

    private void JoinBtn_Clicked(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object> { { "UserId", UserId } };
        Shell.Current.GoToAsync("JoinRosary", navigationParameter);
    }

    private void AdminBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("AdminPage");
    }

    private async void UserEditTapped(object sender, EventArgs e)
    {
        var popup = new EditUserPopup("", "", "");
        var wynik = await this.ShowPopupAsync(popup, new PopupOptions
        {
            Shape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10),
                StrokeThickness = 0
            },
            Shadow = null
        });

        if (wynik is EditUserResult dane)
        {
            await DisplayAlertAsync("Sukces", $"Imię: {dane.Imie}, Nazwisko: {dane.Nazwisko}", "OK");
            // Tutaj możesz dodać logikę do aktualizacji danych użytkownika
        }
    }
}