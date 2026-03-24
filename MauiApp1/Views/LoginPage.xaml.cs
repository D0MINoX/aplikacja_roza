using MauiApp1.Services;

namespace MauiApp1;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    private bool _isBusy;
    public LoginPage(AuthService authService)
    {
        _authService = authService;
        CheckLoginStatus();
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isBusy)
            return;

        _isBusy = true;
        try
        {
            bool success = await _authService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

            if (success)
            {
                await DisplayAlertAsync("Sukces", "Zalogowano!", "OK");
                _isBusy = false;

                await Shell.Current.GoToAsync("Profile");
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Nieprawidłowe dane", "OK");
                _isBusy = false;
            }

        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Błąd techniczny", ex.Message, "OK");
        }
    }
    private async void CheckLoginStatus()
    {
        bool hasToken = await _authService.CheckAndSetTokenAsync();
        if (hasToken)
        {
            await Shell.Current.GoToAsync("Profile");
        }
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Register");
    }
}