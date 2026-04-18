using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace MauiApp1.Views;

public partial class ErrorManagementPage : ContentPage
{
    private readonly ErrorService _errorService;

    public ErrorManagementPage(ErrorService errorService)
    {
        InitializeComponent();
        _errorService = errorService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await LoadErrors();
        
   
         
    }

    private async Task LoadErrors()
    {
        try
        {
            var result = await _errorService.GetAllErrorsAsync();
            if (result.isSuccess)
            {
               
                ErrorsList.ItemsSource = result.Data; // Zaktualizowanie elementów CollectionView
            }
            else
            {
                await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Błąd", $"Wystąpił błąd podczas ładowania danych: {ex.Message}", "OK");
        }
    }



    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var error = button?.CommandParameter as ErrorReport;

        if (error != null)
        {
            // Tworzymy sztywną listę statusów dla bieżącego zgłoszenia
            error.AvailableStatuses = new List<string>
        {
            "Nowe",
            "Odebrane",
            "Zamknięte"
        };

            // Przełącz do trybu edycji
            error.IsEditing = true;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var error = button?.CommandParameter as ErrorReport;

        if (error == null) return;

        bool confirm = await DisplayAlertAsync("Potwierdzenie",
            $"Czy na pewno zapisać zmiany dla zgłoszenia o ID {error.Id}?", "Tak", "Nie");

        if (confirm)
        {
            var result = await _errorService.UpdateErrorStatusAsync(error.Id, error.Status.ToString());

            if (result.isSuccess)
            {
                await DisplayAlertAsync("INFO", "Pomyślnie zaktualizowano status zgłoszenia.", "OK");
                error.IsEditing = false; // Wyłączenie trybu edycji
                await LoadErrors();
            }
            else
            {
                await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");
            }
        }
        else
        {
            error.IsEditing = false; // Anulowanie edycji
        }
    }
}