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

    private async void SendMessage_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("SendMessage");
    }

    private async void ViewGroups_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ViewGroups");
    }
}