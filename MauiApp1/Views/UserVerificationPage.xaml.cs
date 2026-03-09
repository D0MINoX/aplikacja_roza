using MauiApp1.Models;
namespace MauiApp1.Views;

public partial class UserVerificationPage : ContentPage
{
	public UserVerificationPage()
	{
		InitializeComponent();
        LoadDummyData();
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
    private void LoadDummyData()
    {
        var mockUsers = new List<UserInfo>
        {
            new UserInfo { Id = 1, Name = "Jan Kowalski", Email = "jan.k@op.pl", IsVerified = true },
            new UserInfo { Id = 2, Name = "Anna Nowak", Email = "anowak@gmail.com", IsVerified = false },
            new UserInfo { Id = 3, Name = "Piotr Zieliński", Email = "piotrek.z@wp.pl", IsVerified = false },
            new UserInfo { Id = 4, Name = "Maria Woźniak", Email = "m.wozniak@onet.pl", IsVerified = true },
            new UserInfo { Id = 5, Name = "Tomasz Mazur", Email = "tomasz.m@interia.pl", IsVerified = false }
        };

       
        UsersList.ItemsSource = mockUsers;
    }
}