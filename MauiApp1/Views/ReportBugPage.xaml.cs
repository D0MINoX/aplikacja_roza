using MauiApp1.Models;
using MauiApp1.Services;


namespace MauiApp1.Views;

public partial class ReportBugPage : ContentPage
{
    private readonly ErrorService _errorService;
    public ReportBugPage(ErrorService errorService)
	{
        _errorService = errorService;
		InitializeComponent();
	}

	private async void SendReport_Tapped(object sender, TappedEventArgs e)
	{
        
        string errorMessage = PageName.Text + ": " + Description.Text;
        string? userPhone = Phone.Text; 

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            await DisplayAlertAsync("Błąd", "Musisz podać opis błędu!", "OK");
            return;
        }

        ErrorReport errorReport = new ErrorReport
        {
            ErrorMessage = errorMessage,
            UserPhone = userPhone
        };

        var response = await _errorService.SubmitErrorAsync(errorReport);

        if (response.isSuccess)
        {
            await DisplayAlertAsync("Sukces", "Twoje zgłoszenie zostało wysłane. Dziękujemy za pomoc", "OK");
        }
        else
        {
            string error =  response.ErrorMessage;
            await DisplayAlertAsync("Błąd", $"Coś poszło nie tak: {error}", "OK");
        }
    }
    private async void Back_Tapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("..");
    }
}