using CommunityToolkit.Maui.Views;
using MauiApp1.Models;

namespace MauiApp1.Components;

public partial class EditUserPopup : Popup<EditUserResult>
{
    private string _name;
    private string _surname;
    private string _email;

    public EditUserPopup(string name, string surname, string email)
    {
        InitializeComponent();
        NameEntry.Text = name;
        SurnameEntry.Text = surname;
        EmailEntry.Text = email;
    }

    private async void CloseTapped(object sender, EventArgs e)
        => await CloseAsync(null);

    private async void SaveTapped(object sender, EventArgs e)
    {
        EditUserResult result = new EditUserResult
        {
            Name = NameEntry.Text?.Trim() ?? "",
            Surname = SurnameEntry.Text?.Trim() ?? "",
            Email = EmailEntry.Text?.Trim() ?? ""
        };
        await CloseAsync(result);
    }
}