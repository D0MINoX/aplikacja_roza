namespace MauiApp1;

public partial class BottomMenu : ContentView
{
	public BottomMenu()
	{
		InitializeComponent();
	}
    private async void HomeButton_Tapped(object sender, TappedEventArgs e)
    {
        var current = Shell.Current?.CurrentState?.Location?.ToString() ?? "";

        if (current.Contains("Home", StringComparison.OrdinalIgnoreCase))
            return;

        await Shell.Current.GoToAsync("Home");
    }
    private async void MeditationsButton_Tapped(object sender, TappedEventArgs e)
    {
        var current = Shell.Current?.CurrentState?.Location?.ToString() ?? "";

        if (current.Contains("RosaryMeditations", StringComparison.OrdinalIgnoreCase))
            return;

        await Shell.Current.GoToAsync("RosaryMeditations");
    }
    private async void SettingsButton_Tapped(object sender, TappedEventArgs e)
    {
        var current = Shell.Current?.CurrentState?.Location?.ToString() ?? "";

        if (current.Contains("Settings", StringComparison.OrdinalIgnoreCase))
            return;

        await Shell.Current.GoToAsync("Settings");
    }
}