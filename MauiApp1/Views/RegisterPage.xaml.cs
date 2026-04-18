using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.RegularExpressions;

namespace MauiApp1;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly ParishService _parishService;
    private List<Parish> parishes;
    private bool _isBusy;

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
        _isBusy = false;
        string selectedParish = Preferences.Get("ParishName", "Nie wybieraj żadnej parafi");
        if (selectedParish == "Nie wybieraj żadnej parafi")
        {
            ParishLabel.Text = "Wybierz Swoją parafie";
            if (Application.Current.Resources.TryGetValue("FadedText", out var value))
            {
                var color = (Color)value;
                ParishLabel.TextColor = color;
            }
        }
        else
        {
            ParishLabel.Text = selectedParish;
            if (Application.Current.Resources.TryGetValue("Text", out var value))
            {
                var color = (Color)value;
                ParishLabel.TextColor = color;
            }
        }
    }

    private async Task LoadParish()
    {
        var result = await _parishService.AllParish();
        if (result.isSuccess)
        {
            parishes = result.Data;
        }
        else
        {
            await DisplayAlertAsync("Błąd", $"Nie można załadować parafii: {result.ErrorMessage}", "OK");
        }
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text;
        string surname = SurnameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string passwordRetype = PasswordRetype.Text;
        int selectedParish = Preferences.Get("ParishId", 0);
        Preferences.Remove("ParishId");
        Preferences.Remove("ParishName");

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRetype))
        {
            await DisplayAlertAsync("Błąd", "Każede pole musi zostać wypełnione", "OK");
            return;
        }
        else if (!AcceptTermsCheckBox.IsChecked)
        {
            await DisplayAlertAsync("Błąd", "Tworzenie konta wymaga akceptacji regulaminu", "OK");
            return;
        }
        else if (!ValidateEmail(email.Trim()))
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
            string publicIp;
            try
            {
                using var client = new HttpClient();
                publicIp = await client.GetStringAsync("https://api.ipify.org");
                publicIp=publicIp.Trim();
            }
            catch
            {
                await DisplayAlertAsync("Błąd", "Błąd podczas pobierania IP", "OK");
                    return;
            }
            bool isSuccess;
            if (selectedParish == 0)
                isSuccess = await _authService.RegisterAsync(name, surname, email, password, null,publicIp);
            else
                isSuccess = await _authService.RegisterAsync(name, surname, email, password, selectedParish,publicIp);

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

    private async void ParishTapped(object sender, EventArgs e)
    {
        if (_isBusy)
            return;

        _isBusy = true;
        LoadParish();

        if (parishes == null || parishes.Count < 1)
        {
            _isBusy = false;
            return;   
        }

        await this.ShowPopupAsync(new ParishPickerPopup(parishes));

        _isBusy = false;
    }

    private async void OpenRegulamin_Tapped(object sender, EventArgs e)
    {

        await Launcher.Default.OpenAsync("https://info.rosaryapi.pl/#regulamin");
    }

    private async void OpenPrywatnosc_Tapped(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync("https://info.rosaryapi.pl/#prywatnosc");
    }
}