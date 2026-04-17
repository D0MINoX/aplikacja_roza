using CommunityToolkit.Maui.Views;
using MauiApp1.Models;

namespace MauiApp1.Components;

public partial class EditPasswordPopup : Popup<EditPasswordResult>
{

    public EditPasswordPopup()
    {
        InitializeComponent();
    }

    private async void CloseTapped(object sender, EventArgs e)
        => await CloseAsync(null);

    private async void SaveTapped(object sender, EventArgs e)
    {
        if (NewPasswordEntry.Text == NewPasswordRetypeEntry.Text)
        {
            EditPasswordResult result = new EditPasswordResult
            {
                OldPassword = OldPasswordEntry.Text,
                NewPassword = NewPasswordEntry.Text
            };
            
            await CloseAsync(result);
        }
        else
        {
            ErrorLabel.Text = "Hasła nie są takie same. Proszę spróbować ponownie.";
            ErrorLabel.IsVisible = true;
        }
    }
}