using MauiApp1.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MauiApp1;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    public RegisterPage(AuthService authService)
	{
        _authService = authService;
		InitializeComponent();
        
	}
	private async void Register_Clicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text;
        string surname = SurnameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string passwordRetype = PasswordRetype.Text;
        if (name=="" || surname=="" || email=="" || password=="" || passwordRetype=="")
        {
            await DisplayAlertAsync("Błąd", "Każede pole musi zostać wypełnione", "OK");
            return;
        }
        if (!ValidateEmail(email.Trim()))
        {
            await DisplayAlertAsync("Błąd", "Podaj poprawny adres e-mail", "OK");
            return;
        }
        else if(password!=passwordRetype)
        {
            await DisplayAlertAsync("Błąd", "Hasła są różne", "OK");
            return;

        }
        else
        {
           
            bool isSuccess = await _authService.RegisterAsync(name, surname, email, password);
        
      

            if (isSuccess)
            {
               await DisplayAlertAsync("Sukces", "Konto zostało utworzone!", "OK");
               await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Rejestracja nie powiodła się. Może ten użytkownik już istnieje?", "OK");
            }
        }

    }
    public static bool ValidateEmail(string email)
    {
       
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}