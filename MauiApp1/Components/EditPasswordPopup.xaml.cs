using CommunityToolkit.Maui.Views;

namespace MauiApp1.Components;

public partial class EditPasswordPopup : Popup
{

    public EditPasswordPopup()
    {
        InitializeComponent();
    }

    private void CloseTapped(object sender, EventArgs e)
        => CloseAsync();

    private async void SaveTapped(object sender, EventArgs e)
    {
        if (NewPasswordEntry.Text == NewPasswordRetypeEntry.Text)
        {
            //TODO: Save changes to user profile here
            
            await CloseAsync();
        }
        else
        {
            ErrorLabel.Text = "Hasła nie są takie same. Proszę spróbować ponownie.";
            ErrorLabel.IsVisible = true;
        }
    }
}