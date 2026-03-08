using MauiApp1.Models;
using Microsoft.Maui.Controls.Shapes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1;


public partial class JoinRosaryPage : ContentPage, IQueryAttributable
{
    private readonly RosaryService _rosaryService;
    private  int _userId;
	public JoinRosaryPage(RosaryService rosaryService)
	{
		InitializeComponent();
        _rosaryService = rosaryService;
       
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RosariesShow();
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("UserId", out var value) && value is int id)
        {
            _userId = id;
        }
    }
    private async void RosariesShow()
    {
       
            List<RosaryInfo> rosaryInfos = await _rosaryService.GetAllRosariesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RosariesContainer.Children.Clear(); // Czyścimy listę
               
                    foreach (var rosary in rosaryInfos)
                    {
                        try
                        {
                            var border = CreateRosaryCard(rosary);
                            RosariesContainer.Children.Add(border);
                        }
                        catch (Exception ex)
                        {
                            // Debugowanie, jeśli zasób "Kafelki" nadal robi problem
                            System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                        }
                    }
                
            });

        }
    private int _selectedRosaryId = -1;
    private Border CreateRosaryCard(RosaryInfo rosary)
    {

        var colorKafelki = (Color)Application.Current.Resources["Kafelki"];
        var colorMenu = (Color)Application.Current.Resources["Menu"];
        var colorOutline = (Color)Application.Current.Resources["Outline"];
        var colorText = (Color)Application.Current.Resources["Text"];
        var border = new Border
        {
            Padding = new Thickness(15),
            BackgroundColor = colorKafelki, 
            Stroke = colorOutline,        
            StrokeThickness = 2,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Margin = new Thickness(0, 5)
        };
        var tapGesture = new TapGestureRecognizer { CommandParameter = rosary.Id };

        tapGesture.Tapped += (s, e) =>
        {
        
            foreach (var child in RosariesContainer.Children)
            {
                if (child is Border b) b.BackgroundColor = colorKafelki;
            }

     
            border.BackgroundColor = colorMenu; 

           
            _selectedRosaryId = rosary.Id;
            
        };
        border.GestureRecognizers.Add(tapGesture);
        var label = new Label
        {
            Text = rosary.Name,
            TextColor = colorText,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18
        };
        border.Content = label;

       
        return border;

    }

    private async void JoinButton_Tapped(object sender, TappedEventArgs e)
    {
        bool isSuccess = await _rosaryService.JoinRosaryAsync(_userId, _selectedRosaryId);
        if (isSuccess)
        {
            await DisplayAlertAsync("INFO", "Dołączono do róży nr. " + _selectedRosaryId + "poczekaj na zatwierdzenie przez Zelatora", "OK");
            await Shell.Current.GoToAsync("//Home");
        }
        else
        {
            await DisplayAlertAsync("ALERT", "błąd dołączenia do róży", "OK");
        }
        
       
    }
}