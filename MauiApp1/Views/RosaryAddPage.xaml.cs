using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace MauiApp1.Views;

[QueryProperty(nameof(UserRole), "UserRole")]
[QueryProperty(nameof(UserId), "UserId")]
public partial class RosaryAddPage : ContentPage
{
    public int UserRole { get; set; }
    public int UserId { get; set; }
    private readonly AdminService _adminService;
    private readonly ParishService _parishService;

    public RosaryAddPage(AdminService adminService, ParishService parishService)
	{
		_adminService = adminService;
        _parishService = parishService;
		InitializeComponent();
	}
	protected override void OnAppearing()
	{
        base.OnAppearing();
        LoadUsers();
       
        LoadParishes();
      
	
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
    private async void LoadParishes()
    {
        if (UserRole == 0)
        {
            var result= await _parishService.AllParish();
            if (result.isSuccess)
            {

                var namesList = result.Data.Select(parish => $"{parish.Id} {parish.Name}").ToList();
                ParishPicker.ItemsSource = namesList;

            }
            else
            {

                await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");

                ParishPicker.ItemsSource = null;
            }
        }
        else
        {
            var result=  await _parishService.GetUserParish(UserId);
            if (result.isSuccess)
            {

                var namesList =new List<Parish> { result.Data }.Select(parish=>$"{parish.Id} {parish.Name}").ToList();
                ParishPicker.ItemsSource = namesList;
                ParishPicker.SelectedIndex=0;

            }
            else
            {

                await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");

                ParishPicker.ItemsSource = null;
            }
        }


        
    }
    private async void Register_Clicked(object sender, EventArgs e)
    {
       string Name= NameEntry.Text;
        int Parish = int.Parse(ParishPicker.SelectedItem.ToString().Split(" ")[0]);
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