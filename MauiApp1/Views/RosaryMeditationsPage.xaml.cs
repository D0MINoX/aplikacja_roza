

using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;

namespace MauiApp1;

public partial class RosaryMeditationsPage : ContentPage
{
    public int date;
    public string Link;
    public MeditationsService _meditationService;
    private bool _isBusy = false;
    public RosaryMeditationsPage(MeditationsService meditationService)
    {
        InitializeComponent();

        _meditationService = meditationService;

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        _isBusy = true; // Zaciągamy hamulec ręczny na czas ładowania

        // 1. Odczytujemy dane
        date = Preferences.Default.Get("LastDate", 1);
        string savedGroup = Preferences.Default.Get("LastGroup", "Radosne");
        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");

        GroupLabel.Text = savedGroup;
        DetailLabel.Text = savedMystery;

        _isBusy = false; // Puszczamy hamulec

        UpdateDate(); // Teraz robimy pierwsze pobranie z API
    }

    private async void UpdateDate()
    {
        if (_isBusy) return; // Jeśli trwa ustawianie pickerów, nie rób nic
        try
        {
            DateLabel.Text = "dzień " + date;

            // Zapisuj do pamięci tylko jeśli wartości są poprawne
            Preferences.Default.Set("LastDate", date);


            // Ładowanie z API
            string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            if (selectedMystery != "")
            {
                MeditationLabel.Text = "Ładowanie ....";
                
                var data = await _meditationService.GetMeditationData(this.date, selectedMystery);
                MeditationLabel.Text = data.Content ?? "Brak rozważania";
                if (data.Link == null) {
                    YTTile.IsVisible = false;
                }
                else
                {
                    YTTile.IsVisible = true;
                    Link = data.Link;
                }
                
            }
        }
        catch (Exception ex)
        {
            MeditationLabel.Text = "Błąd połączenia";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private async void PlayTapped(object sender, EventArgs e)
    {
        //do zrobienia link zalezny od rozwazania
        await Browser.Default.OpenAsync(
        new Uri(Link),
        BrowserLaunchMode.SystemPreferred);
    }

    private async void PreviousTapped(object sender, EventArgs e)
    {
        if (--date < 1) date = 31;
        UpdateDate();
    }

    private async void NextTapped(object sender, EventArgs e)
    {

        if (++date > 31) date = 1;
        UpdateDate();
    }

    private async void MeditationTapped(object sender, EventArgs e)
    {
        string textToSend = MeditationLabel.Text;

        if (string.IsNullOrWhiteSpace(textToSend) || textToSend == "Brak rozważania")
            return;

        var navigationParameter = new Dictionary<string, object>
    {
        { "MeditationContent", textToSend }
    };
        await Shell.Current.GoToAsync("FullMeditation", navigationParameter);
    }

    public async void GroupTapped(object sender, EventArgs e)
    {
        if (_isBusy)
            return;
        _isBusy = true;
        await this.ShowPopupAsync(new PickerPopup("Group"));
    }

    public async void DetailsTapped(object sender, EventArgs e)
    {
        if (_isBusy)
            return;
        _isBusy = true;
        var group = Preferences.Default.Get("LastGroup", "");
        await this.ShowPopupAsync(new PickerPopup(group));
    }
}
