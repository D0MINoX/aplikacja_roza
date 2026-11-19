namespace MauiApp1;

public partial class LoginRequiredPage : ContentPage
{
    public LoginRequiredPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Login");
    }

    private async void OnRegisterClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Register");
    }
}
