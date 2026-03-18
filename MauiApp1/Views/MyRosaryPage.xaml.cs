namespace MauiApp1;

public partial class MyRosaryPage : ContentPage
{
    public MyRosaryPage()
    {
        InitializeComponent();
    }

  

    private async void Messages_Tapped(object sender, TappedEventArgs e)
    {
          await Shell.Current.GoToAsync("Messages");
    }

    private async void ViewGroups_Tapped(object sender, TappedEventArgs e)
    {
        // await Shell.Current.GoToAsync("ViewGroups");
    }

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Login");
    }
}