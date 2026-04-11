using MauiApp1.Models;
using Microsoft.Maui.Controls.Shapes;

namespace MauiApp1;

[QueryProperty(nameof(UserId), "UserId"), QueryProperty(nameof(Parish), "Parish")]
public partial class JoinRosaryPage : ContentPage
{
    private readonly RosaryService _rosaryService;
    public int UserId { get; set; }
    public int Parish { get; set; }
    private int _selectedRosaryId = -1;

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
 
    private async void RosariesShow()
    {
        await DisplayAlertAsync("INFO", UserId.ToString(), "OK");
        List<RosaryInfo> rosaryInfos = await _rosaryService.GetAvailableRosariesAsync(Parish);
        MainThread.BeginInvokeOnMainThread(() =>
        {   
            RosariesContainer.BatchBegin();
            try
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
                        // Debugowanie, jeśli zasób "Primary" nadal robi problem
                        System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                    }
                }
            }
            finally
            {
                RosariesContainer.BatchCommit();
            }
        });
    }

    private Border CreateRosaryCard(RosaryInfo rosary)
    {
        var colorPrimary = (Color)Application.Current.Resources["Primary"];
        var colorMenu = (Color)Application.Current.Resources["Secondary"];
        var borderStyle = (Style)Application.Current.Resources["ListElement"];

        var border = new Border
        {
            Style = borderStyle
        };

        var tapGesture = new TapGestureRecognizer { CommandParameter = rosary.Id };
        tapGesture.Tapped += (s, e) =>
        {
            foreach (var child in RosariesContainer.Children)
            {
                if (child is Border b) b.BackgroundColor = colorPrimary;
            }

            border.BackgroundColor = colorMenu;

            _selectedRosaryId = rosary.Id;
        };
        border.GestureRecognizers.Add(tapGesture);

        var label = new Label
        {
            Text = rosary.Name
        };
        border.Content = label;

        return border;
    }

    private async void JoinButton_Tapped(object sender, TappedEventArgs e)
    {
        // I pasowało by jeszcze przy zapisaniu grupy zapisać parafię
        if (_selectedRosaryId == -1)
        {
            await DisplayAlertAsync("Błąd", "Najpierw wybierz różę z listy!", "OK");
            return;
        }
        var result = await _rosaryService.JoinRosaryAsync(UserId, _selectedRosaryId);

        if (result.IsSuccess)
        {
            await DisplayAlertAsync("INFO",
                $"Wysłano prośbę do róży nr {_selectedRosaryId}. Poczekaj na zatwierdzenie przez Zelatora.",
                "OK");

            await Shell.Current.GoToAsync("//Home");
        }
        else
        {
            // Wyświetlamy konkretny błąd otrzymany z API
            await DisplayAlertAsync("BŁĄD API", result.ErrorMessage, "OK");
        }
    }

    private async void SelectParish_Tapped(object sender, TappedEventArgs e)
    {
        var navigationParameter = new Dictionary<string, object> { { "UserId", UserId } };
        await Shell.Current.GoToAsync("SelectParish", navigationParameter);
    }
}