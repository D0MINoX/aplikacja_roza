namespace MauiApp1;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
        CheckLoginStatus();
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {

        try
        {
            bool success = await _authService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

            if (success)
            {
                await DisplayAlertAsync("Sukces", "Zalogowano!", "OK");
                string token = "twoj_wygenerowany_token_jwt"; // Pobierasz go z AuthService [cite: 2026-01-12]

                await Shell.Current.GoToAsync("Profile");
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Nieprawidłowe dane", "OK");
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
}