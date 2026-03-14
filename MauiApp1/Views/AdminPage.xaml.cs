namespace MauiApp1.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
	}

    private async void RosaryMenagement_Tapped(object sender, TappedEventArgs e)
    {
		await Shell.Current.GoToAsync("AdminRosaries");
    }
    private async void RosaryAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RosaryAdd");
    }
}