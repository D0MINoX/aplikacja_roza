using MauiApp1.Services;


namespace MauiApp1.Views;

public partial class RosaryAddPage : ContentPage
{
    private readonly AdminService _adminService;
    public RosaryAddPage(AdminService adminService)
	{
		_adminService = adminService;
		InitializeComponent();
	}
	protected override void OnAppearing()
	{
        LoadUsers();
	
	}
    private async void LoadUsers()
    {
        var result = await _adminService.AdminZelators();

        if (result.isSuccess)
        {

            var namesList = result.Data
         .Select(user => $"{user.UserId} {user.UserName} {user.UserSurname}") 
         .ToList();
            ZelatorPicker.ItemsSource = namesList;

        }
        else
        {

            await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");

            ZelatorPicker.ItemsSource = null;
        }
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
       string Name= NameEntry.Text;
        string Parish = ParishEntry.Text;
        int zelator = int.Parse(ZelatorPicker.SelectedItem.ToString().Split(" ")[0]);
        bool isSuccess = await _adminService.RegisterAsync(Name,Parish,zelator);
        if (isSuccess)
        {
            await DisplayAlertAsync("Sukces", "Róża została utworzona!", "OK");

        }
        else
        {

            await DisplayAlertAsync("Błąd", "Rejestracja nie powiodła się. Może róża już istnieje", "OK");

            ZelatorPicker.ItemsSource = null;
        }
        
    }
}