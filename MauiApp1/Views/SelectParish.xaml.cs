using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.Views;

[QueryProperty(nameof(UserId), "UserId")]
public partial class SelectParish : ContentPage
{
    private readonly ParishService _parishService;
    private bool _loadingParishes;
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
        if (_loadingParishes)
            return;

        _loadingParishes = true;
        try
        {
            while (true)
            {
                var result = await _parishService.AllParish();
                if (result.isSuccess && result.Data is not null)
                {
                    ParishContainer.Children.Clear();
                    selectedParish = 0;
                    foreach (var parish in result.Data)
                        ParishContainer.Children.Add(CreateRosaryCard(parish));
                    return;
                }

                bool retry = await DisplayAlertAsync("Błąd połączenia",
                    "Nie udało się pobrać listy parafii.", "Spróbuj ponownie", "Anuluj");
                if (!retry)
                    return;
            }
        }
        finally
        {
            _loadingParishes = false;
        }
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
