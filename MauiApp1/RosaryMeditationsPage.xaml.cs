namespace MauiApp1;

public partial class RosaryMeditationsPage : ContentPage
{
    public int date;
    public MeditationsService _meditationService;
    public RosaryMeditationsPage(MeditationsService meditationService)
    {
        InitializeComponent();
        date = 1;
        _meditationService = meditationService;
        GroupPicker.SelectedItem = "Radosne";
        DetailPicker.SelectedItem = "Zwiastowanie Najświętszej Maryi Pannie";
        UpdateDate();
    }

    private async void UpdateDate()
    {
        DateLabel.Text = "dzień " + date;
        var picker = DetailPicker;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            string description = await _meditationService.GetOnlyDescription(this.date, picker.SelectedItem.ToString());
            MeditationLabel.Text = description ?? "Brak rozważania";

        }
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
        var group = GroupPicker.SelectedItem.ToString();
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

    private async void OnDetailChanged(object sender, EventArgs e)
    {
        UpdateDate();

    }
}
