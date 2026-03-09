namespace MauiApp1.Views;

public partial class RosaryMenagementPage : ContentPage
{
	public RosaryMenagementPage()
	{
		InitializeComponent();
	}

    private async void UserVerification_Tapped(object sender, TappedEventArgs e)
    {
		await Shell.Current.GoToAsync("UserVerification");
    }
}