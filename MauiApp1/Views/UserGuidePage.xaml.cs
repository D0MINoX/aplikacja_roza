namespace MauiApp1.Views;

public partial class UserGuidePage : ContentPage
{
    public UserGuidePage()
    {
        InitializeComponent();
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", false);
    }
}
