namespace MauiApp1;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {

        try
        {
            bool success = await _authService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

            if (success)
            {
                await DisplayAlertAsync("Sukces", "Zalogowano!", "OK");
                // Przejdź do głównej strony
                await Shell.Current.GoToAsync("//MainPage");
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
}