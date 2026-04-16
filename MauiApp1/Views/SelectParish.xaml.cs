using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.Views;

[QueryProperty(nameof(UserId), "UserId")]
public partial class SelectParish : ContentPage
{
    private readonly ParishService _parishService;
    public int UserId { get; set; }
    private int selectedParish {  get; set; }
    public SelectParish(ParishService parishService)
	{
		InitializeComponent();
        _parishService = parishService;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RosariesShow();
    }

    private async void RosariesShow()
    {
        //await DisplayAlertAsync("INFO", UserId.ToString(), "OK");
        var res = await _parishService.AllParish();
        List<Parish> allParishes = res.Data;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ParishContainer.BatchBegin();
            try
            {
                ParishContainer.Children.Clear();

                foreach (var parish in allParishes)
                {
                    try
                    {
                        var border = CreateRosaryCard(parish);
                        ParishContainer.Children.Add(border);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                    }
                }
            }
            finally
            {
                ParishContainer.BatchCommit();
            }
        });
    }

    private Border CreateRosaryCard(Parish parish)
    {
        var colorPrimary = (Color)Application.Current.Resources["Primary"];
        var colorMenu = (Color)Application.Current.Resources["Secondary"];
        var borderStyle = (Style)Application.Current.Resources["ListElement"];

        var border = new Border
        {
            Style = borderStyle
        };

        var tapGesture = new TapGestureRecognizer { CommandParameter = parish.Id };
        tapGesture.Tapped += (s, e) =>
        {
            foreach (var child in ParishContainer.Children)
            {
                if (child is Border b) b.BackgroundColor = colorPrimary;
            }

            border.BackgroundColor = colorMenu;

            selectedParish = parish.Id;
        };
        border.GestureRecognizers.Add(tapGesture);

        var label = new Label
        {
            Text = parish.Name
        };
        border.Content = label;

        return border;
    }

    private async void Select_Tapped(object sender, TappedEventArgs e)
    {
        if (selectedParish == 0)
        {
            await DisplayAlertAsync("Błąd", "Najpierw wybierz parafię z listy!", "OK");
            return;
        }

        var navigationParameter = new Dictionary<string, object> { { "UserId", UserId }, { "Parish", selectedParish } };
        await Shell.Current.GoToAsync("JoinRosary", navigationParameter);
    }
}