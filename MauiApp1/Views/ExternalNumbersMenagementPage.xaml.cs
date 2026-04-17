using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.RegularExpressions;

namespace MauiApp1.Views;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class ExternalNumbersMenagementPage : ContentPage
{
    private readonly MessagesService _messegesService;
    private readonly AdminService _adminService;
    public int RosaryId { get; set; }

    public ExternalNumbersMenagementPage(MessagesService messegesService, AdminService adminService)
    {
        InitializeComponent();
        _messegesService = messegesService;
        _adminService = adminService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadNumbers();
      
    }

    private async void LoadNumbers()
    {
        try
        {
            var result = await _messegesService.getAdminExternalNumbers(RosaryId);

                
                NumbersList.ItemsSource = result;
          
        }catch(Exception ex)
        {
            await DisplayAlertAsync("Błąd", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button.BindingContext as ExternalNumber;

        bool confirm = await DisplayAlertAsync("UWAGA", $"Czy na pewno usunąć numer użytkownika {user.Name} {user.Surname}?", "USUŃ", "Anuluj");
        if (confirm)
        {
            await _adminService.DeleteExternalNumber(user.Id, RosaryId);
            LoadNumbers();
        }
    }
    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button?.CommandParameter as ExternalNumber;

        if (user != null)
            user.IsEditing = true;
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var user = (sender as Button).CommandParameter as ExternalNumber;

        bool confirm = await DisplayAlertAsync("Potwierdzenie",
            $"Czy na pewno zapisać zmiany dla użytkownika {user.Name} {user.Surname}?", "Tak", "Anuluj");

        if (confirm)
        {
            bool isSuccess = await _adminService.UpdateExternalMember(
                user.Id,
                user.Name,
                user.Surname,
                user.PhoneNumber) ;
            if (isSuccess)
            {
                await DisplayAlertAsync("INFO", "Pomyślnie zmieniono dane użytkownika", "OK");

            }
            else
            {
                await DisplayAlertAsync("Błąd", "Wystąpił błąd przy zmianie danych", "OK");
            }
            user.IsEditing = false;
            LoadNumbers(); 
        }
        else
        {
            user.IsEditing = false; 
        }
    }
    private async void OpenPrywatnoscClicked(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync("https://info.rosaryapi.pl/#prywatnosc");
    }
    private async void RegisterClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();
        string surname = SurnameEntry.Text?.Trim();
        string phone = PhoneEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(phone))
        {
            await DisplayAlertAsync("Błąd", "Każde pole musi zostać wypełnione", "OK");
            return;
        }
        else if (!AcceptTermsCheckBox.IsChecked)
        {
            await DisplayAlertAsync("Błąd", "Dodanie numeru wymaga potwierdzenia zgody", "OK");
            return;
        }
        else if (!ValidatePhoneNumber(phone))
        {
            await DisplayAlertAsync("Błąd", "Podaj poprawny numer telefonu", "OK");
            return;
        }
        else
        {
            string publicIp;
            try
            {
                using var client = new HttpClient();
                publicIp = await client.GetStringAsync("https://api.ipify.org");
                publicIp = publicIp.Trim();
            }
            catch
            {
                await DisplayAlertAsync("Błąd", "Błąd podczas pobierania IP", "OK");
                return;
            }
            bool isSuccess = await _adminService.RegisterExternalMemberAsync(
                name,
                surname,
                phone,
                RosaryId,
                publicIp
            );

            if (isSuccess)
            {
                await DisplayAlertAsync("Sukces", "Numer został dodany!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Dodanie numeru nie powiodło się", "OK");
            }
        }
    }
    private void OnFabClicked(object sender, EventArgs e)
    {
        if (newNumber.IsVisible)
        {
            newNumber.IsVisible = false;
            FabButton.Text = "+";
        }
        else
        {
            newNumber.IsVisible = true;
            FabButton.Text = "-";
        }
    }
    private bool ValidatePhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        phone = phone.Replace(" ", "").Replace("-", "");

        // prosty wariant dla polskich numerów
        return Regex.IsMatch(phone, @"^(\+48)?\d{9}$");
    }
}