using CommunityToolkit.Maui.Views;
using MauiApp1.Models;

namespace MauiApp1.Components;

public partial class EditUserPopup : Popup<EditUserResult>
{
    public EditUserPopup(string name, string surname, string email)
    {
        InitializeComponent();
        NameEntry.Text = name;
        SurnameEntry.Text = surname;
        EmailEntry.Text = email;
    }

    private void CloseTapped(object sender, EventArgs e)
        => CloseAsync();

    private void SaveTapped(object sender, EventArgs e)
    {
        CloseAsync(new EditUserResult(
            NameEntry.Text?.Trim() ?? "",
            SurnameEntry.Text?.Trim() ?? "",
            EmailEntry.Text?.Trim() ?? ""
        ));
    }
}