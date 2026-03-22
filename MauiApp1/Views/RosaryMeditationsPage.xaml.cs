

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
        int savedGroupIdx = Preferences.Default.Get("LastGroupIndex", 0);
        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");

        // 2. Ustawiamy Grupę i ładujemy listę
        GroupPicker.SelectedIndex = savedGroupIdx;
        Mysteries();

        // 3. Ustawiamy konkretną tajemnicę (SelectedItem zamiast Index)
        if (DetailPicker.ItemsSource is List<string> list && list.Contains(savedMystery))
        {
            DetailPicker.SelectedItem = savedMystery;
        }
        else
        {
            DetailPicker.SelectedIndex = 0;
        }

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

            if (GroupPicker.SelectedIndex != -1)
                Preferences.Default.Set("LastGroupIndex", GroupPicker.SelectedIndex);

            if (DetailPicker.SelectedItem != null)
                Preferences.Default.Set("LastMystery", DetailPicker.SelectedItem.ToString());

            // Ładowanie z API
            if (DetailPicker.SelectedItem != null)
            {
               
                MeditationLabel.Text = "Ładowanie ....";
                string selectedMystery = DetailPicker.SelectedItem.ToString();
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

    private void OnGroupChanged(object sender, EventArgs e)
    {
        if (_isBusy) return;

        Mysteries();
        // Po zmianie grupy, DetailPicker traci wybór, więc ustawiamy pierwszy
        if (DetailPicker.ItemsSource?.Count > 0)
        {
            DetailPicker.SelectedIndex = 0;
        }
        UpdateDate();
    }

    private void OnDetailChanged(object sender, EventArgs e)
    {
        if (_isBusy) return;
        UpdateDate();
    }
    private void Mysteries()
    {
        var group = GroupPicker.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(group))
        {
            DetailPicker.IsEnabled = false;
            return;
        }

        ThemeManager.SetTheme(group);


        DetailPicker.IsEnabled = true;
        DetailPicker.ItemsSource = group switch
        {
            "Radosne" => new List<string>
                {
                    "Zwiastowanie Najświętszej Maryi Pannie",
                    "Nawiedzenie św. Elżbiety",
                    "Narodzenie Pana Jezusa",
                    "Ofiarowanie Pana Jezusa w świątyni",
                    "Odnalezienie Pana Jezusa w świątyni"
                },
            "Światła" => new List<string>
                {
                    "Chrzest Pana Jezusa w Jordanie",
                    "Objawienie się Pana Jezusa w Kanie Galilejskiej",
                    "Głoszenie Królestwa Bożego i wzywanie do nawrócenia",
                    "Przemienienie na górze Tabor",
                    "Ustanowienie Eucharystii"
                },
            "Bolesne" => new List<string>
                {
                    "Modlitwa Pana Jezusa w Ogrójcu",
                    "Biczowanie Pana Jezusa",
                    "Cierniem ukoronowanie Pana Jezusa",
                    "Dźwiganie krzyża na Kalwarię",
                    "Ukrzyżowanie i śmierć Pana Jezusa"
                },
            "Chwalebne" => new List<string>
                {
                    "Zmartwychwstanie Pana Jezusa",
                    "Wniebowstąpienie Pana Jezusa",
                    "Zesłanie Ducha Świętego",
                    "Wniebowzięcie Najświętszej Maryi Panny",
                    "Ukoronowanie Najświętszej Maryi Panny na Królową Nieba i Ziemi"
                },
            _ => new List<string>()
        };
    }

    public void GroupTapped(object sender, EventArgs e)
    {
        this.ShowPopup(new PickerPopup("Group"));
    }

    public void DetailsTapped(object sender, EventArgs e)
    {
        var group = Preferences.Default.Get("LastGroup", "");
        this.ShowPopup(new PickerPopup(group));
    }
}
