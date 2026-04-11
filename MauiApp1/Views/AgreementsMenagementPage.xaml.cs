using MauiApp1.Services;

namespace MauiApp1.Views;

public partial class AgreementsMenagementPage : ContentPage
{
    private readonly AdminService _adminService;
	public AgreementsMenagementPage(AdminService adminService)
	{
        _adminService = adminService;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
        LoadUsersConsent();
    }

    private async void LoadUsersConsent()
    {
        var result = await _adminService.AdminConsentsShow();

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