using MauiApp1.Models;
using MauiApp1.Services;
namespace MauiApp1.Views;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class UserVerificationPage : ContentPage
{
    private readonly AdminService _adminService;
    public int RosaryId { get; set; }
	public UserVerificationPage(AdminService adminService)
	{
        _adminService = adminService;
        
		InitializeComponent();
      
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (RosaryId != 0)
        {
            LoadUsers(RosaryId);
        }
    }

    private async void OnVerifyClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button.BindingContext as AdminUserView;

        var confirm = await DisplayAlertAsync("Weryfikacja", $"Czy chcesz zweryfikować {user.UserName}?", "Tak", "Nie");
        if (confirm)
        {
             await _adminService.VerifyUser(user.UserId,RosaryId);
       
            // Odśwież listę
            LoadUsers(RosaryId);
        }
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button.BindingContext as AdminUserView;

        bool confirm = await DisplayAlertAsync("UWAGA", $"Czy na pewno usunąć użytkownika {user.UserName}?", "USUŃ", "Anuluj");
        if (confirm)
        {
              await _adminService.DeleteUser(user.UserId,RosaryId);
            LoadUsers(RosaryId);
        }
    }
    private async void LoadUsers(int id)
    {
        var result = await _adminService.AdminUsers(id);

        if (result.isSuccess)
        {
            UsersList.ItemsSource = result.Data;
        }
        else
        {

            await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");

            UsersList.ItemsSource = null;
        }
    }
}