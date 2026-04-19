using MauiApp1.Models;
using MauiApp1.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Views;

[QueryProperty(nameof(UserRole), "UserRole")]
public partial class ChangeUserPrivilagiesPage : ContentPage
{
    private readonly AdminService _adminService;
    
    public int UserRole { get; set; }
    public ChangeUserPrivilagiesPage(AdminService adminService)
	{
        _adminService = adminService;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
            LoadUsers();
        
    }
  
    
    private async void LoadUsers()
    {
        var result = await _adminService.UsersPrivilagiesShow(UserRole);

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
    private void OnEditClicked(object sender, EventArgs e)
    {
        var user = (sender as Button).CommandParameter as AdminUserView;
        var allRoles = new List<RoleOption>
    {
        new RoleOption { Id = 0, Name = "Admin" },
        new RoleOption { Id = 1, Name = "Zelator główny" },
        new RoleOption { Id = 2, Name = "Zelator" },
        new RoleOption { Id = 3, Name = "Członek róży" },
        new RoleOption { Id = 4, Name = "Brak" }
    };
        user.AvailableRoles = allRoles
        .Where(r => r.Id >= this.UserRole)
        .ToList();
        user.IsEditing = true;
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var user = (sender as Button).CommandParameter as AdminUserView;

        bool confirm = await DisplayAlertAsync("Potwierdzenie",
            $"Czy na pewno zapisać zmiany dla użytkownika {user.UserName}?", "Tak", "Anuluj");

        if (confirm)
        {
            bool isSuccess = await _adminService.UpdateUserPermissions(
           user.UserId,
           user.UserRole,
           user.UserCanSendSMS); ;
            if (isSuccess)
            {
                await DisplayAlertAsync("INFO", "Pomyślnie zmieniono rolę użytkownika", "OK");

            }
            else
            {
                await DisplayAlertAsync("Błąd", "Wystąpił błąd przy zmianie roli", "OK");
            }
            user.IsEditing = false;
            LoadUsers(); 
        }
        else
        {
            user.IsEditing = false; 
        }
    }
   
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var user = (sender as Button).CommandParameter as AdminUserView;
        var result = await _adminService.deleteUser(user.UserId);

        if (result.isSuccess)
        {
            LoadUsers();
        }
        else
        {
            await DisplayAlertAsync("Błąd", result.ErrorMessage, "OK");
        }
    }
}