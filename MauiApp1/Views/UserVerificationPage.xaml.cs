using MauiApp1.Models;
using MauiApp1.Services;
namespace MauiApp1.Views;

public partial class UserVerificationPage : ContentPage
{
    private readonly AdminService _adminService;
	public UserVerificationPage(AdminService adminService)
	{
        _adminService = adminService;
		InitializeComponent();
        LoadUsers(2);
    }
    private async void OnVerifyClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button.BindingContext as UserInfo;

        bool confirm = await DisplayAlertAsync("Weryfikacja", $"Czy chcesz zweryfikować {user.Name}?", "Tak", "Nie");
        if (confirm)
        {
            // Wywołaj swoje API: await _adminService.VerifyUser(user.Id);
            user.IsVerified = true;
            // Odśwież listę
        }
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button.BindingContext as UserInfo;

        bool confirm = await DisplayAlertAsync("UWAGA", $"Czy na pewno usunąć użytkownika {user.Name}?", "USUŃ", "Anuluj");
        if (confirm)
        {
            // Wywołaj API: await _adminService.DeleteUser(user.Id);
        }
    }
    private async void LoadUsers(int id)
    {
        var result = await _adminService.AdminUsers(id);

        if (result.isSuccess)
        {
            // Wyświetlamy pobraną listę w CollectionView
            UsersList.ItemsSource = result.Data;
        }
        else
        {
            // Wyświetlamy konkretny błąd z API użytkownikowi
            await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");

            // Opcjonalnie: wyczyść listę, jeśli wystąpił błąd
            UsersList.ItemsSource = null;
        }
    }
}