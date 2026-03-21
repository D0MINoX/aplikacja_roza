using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.RegularExpressions;

namespace MauiApp1;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly ParishService _parishService;
    public RegisterPage(AuthService authService, ParishService parishService)
    {
        _authService = authService;
        _parishService = parishService;
        InitializeComponent();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadParish();

    }

    private async Task LoadParish()
    {
        var result = await _parishService.AllParish();
        if (result.isSuccess)
        {
            ParishPicker.ItemsSource = result.Data;
        }
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text;
        string surname = SurnameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string passwordRetype = PasswordRetype.Text;
        var selectedParish = ParishPicker.SelectedItem as Parish;

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
        else if (password != passwordRetype)
        {
            await DisplayAlertAsync("Błąd", "Hasła są różne", "OK");
            return;

        }
        else
        {
            
                bool isSuccess = await _authService.RegisterAsync(name, surname, email, password,selectedParish?.Id);



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