namespace MauiApp1.Views;

public partial class ReportBugPage : ContentPage
{
	public ReportBugPage()
	{
		InitializeComponent();
	}

	private async void SendReport_Tapped(object sender, TappedEventArgs e)
	{
	}

	private async void Back_Tapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("..");
    }
}