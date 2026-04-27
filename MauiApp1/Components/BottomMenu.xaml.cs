namespace MauiApp1;

public partial class BottomMenu : ContentView
{
    public BottomMenu()
    {
        InitializeComponent();
    }
    private async void HomeButton_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//Home",false);
    }
    private async void MeditationsButton_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RosaryMeditations");
    }
    private async void SettingsButton_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Settings");
    }
}