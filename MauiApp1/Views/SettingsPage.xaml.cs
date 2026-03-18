using MauiApp1.Services;

namespace MauiApp1;

public partial class SettingsPage : ContentPage
{
    private readonly AuthService _authService;
    public SettingsPage(AuthService authService)
    {
        _authService = authService;
        InitializeComponent();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        IsLogged();
    }

    private async void IsLogged()
    {
        bool hasToken = await _authService.CheckAndSetTokenAsync();

        if (hasToken)
        {
            LogIn.IsVisible = false;
            LogOut.IsVisible = true;
        }
        else
        {
            LogIn.IsVisible = true;
            LogOut.IsVisible = false;
        }
    }

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Login");
    }

    private async void Logout_Tapped(object sender, TappedEventArgs e)
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
        IsLogged();
    }

    private async void Profile_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Profile");

    }

    private void ThemeTapped(object sender, TappedEventArgs e)
    {
        ThemeManager.SetMainTheme();
    }
}