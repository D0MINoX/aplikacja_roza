using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
using MauiApp1.Models;
using System.Text.Json;


namespace MauiApp1;

public partial class RosaryMeditationsPage : ContentPage
{
    public int date;
    public string Link;
    public MeditationsService _meditationService;
    private bool _isBusy = false;
    private static readonly Dictionary<string, List<string>> _itemsMap = new()
    {
        ["Group"] = new()
        {
            "Radosne",
            "Światła",
            "Bolesne",
            "Chwalebne"
        },
        ["Radosne"] = new()
        {
            "Zwiastowanie Najświętszej Maryi Pannie",
            "Nawiedzenie św. Elżbiety",
            "Narodzenie Pana Jezusa",
            "Ofiarowanie Pana Jezusa w świątyni",
            "Odnalezienie Pana Jezusa w świątyni"
        },
        ["Światła"] = new()
        {
            "Chrzest Pana Jezusa w Jordanie",
            "Objawienie się Pana Jezusa w Kanie Galilejskiej",
            "Głoszenie Królestwa Bożego i wzywanie do nawrócenia",
            "Przemienienie na górze Tabor",
            "Ustanowienie Eucharystii"
        },
        ["Bolesne"] = new()
        {
            "Modlitwa Pana Jezusa w Ogrójcu",
            "Biczowanie Pana Jezusa",
            "Cierniem ukoronowanie Pana Jezusa",
            "Dźwiganie krzyża na Kalwarię",
            "Ukrzyżowanie i śmierć Pana Jezusa"
        },
        ["Chwalebne"] = new()
        {
            "Zmartwychwstanie Pana Jezusa",
            "Wniebowstąpienie Pana Jezusa",
            "Zesłanie Ducha Świętego",
            "Wniebowzięcie Najświętszej Maryi Panny",
            "Ukoronowanie Najświętszej Maryi Panny na Królową Nieba i Ziemi"
        }
    };
    public RosaryMeditationsPage(MeditationsService meditationService)
    {
        InitializeComponent();

        _meditationService = meditationService;

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        _isBusy = true; 

        // 1. Odczytujemy dane
        date = Preferences.Default.Get("LastDate", 1);
        string savedGroup = Preferences.Default.Get("LastGroup", "Radosne");
        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");

        GroupLabel.Text = savedGroup;
        DetailLabel.Text = savedMystery;

        _isBusy = false; 

        UpdateDate();
 }

    private async void UpdateDate()
    {
        if (_isBusy) return;
        try
        {
            DateLabel.Text = "dzień " + date;
            Preferences.Default.Set("LastDate", date);

            string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            if (string.IsNullOrEmpty(selectedMystery)) return;

            MeditationLabel.Text = "Ładowanie ....";

          
            var localData = await GetMeditationFromLocalFile(this.date, selectedMystery);

            if (localData != null)
            {
                ApplyMeditationData(localData);
                return; 
            }

           
            bool autoDownload = Preferences.Default.Get("AutoDownloadMeditations", false);

            if (autoDownload)
            {
              
                bool downloaded = await DownloadAllMeditationsForMystery(selectedMystery);
                if (downloaded)
                {
          
                    var freshLocalData = await GetMeditationFromLocalFile(this.date, selectedMystery);
                    if (freshLocalData != null)
                    {
                        ApplyMeditationData(freshLocalData);
                        return;
                    }
                }
            }

            var data = await _meditationService.GetMeditationData(this.date, selectedMystery);
            ApplyMeditationData(data);
        }
        catch (Exception ex)
        {
            MeditationLabel.Text = "Błąd połączenia";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private async void PlayTapped(object sender, EventArgs e)
    {
        
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
        await this.ShowPopupAsync(new PickerPopup(_itemsMap["Group"], "Group"));
    }

    public async void DetailsTapped(object sender, EventArgs e)
    {
        if (_isBusy)
            return;
        _isBusy = true;
        var group = Preferences.Default.Get("LastGroup", "");
        await this.ShowPopupAsync(new PickerPopup(_itemsMap[group], "Mystery"));
    }
    private void ApplyMeditationData(LocalMeditation data)
    {
        MeditationLabel.Text = data?.Content ?? "Brak rozważania";
        if (string.IsNullOrEmpty(data?.Link))
        {
            YTTile.IsVisible = false;
        }
        else
        {
            YTTile.IsVisible = true;
            Link = data.Link;
        }
    }
    private async Task<LocalMeditation> GetMeditationFromLocalFile(int day, string mystery)
    {
        try
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, $"meditations_{mystery.GetHashCode()}.json");
            if (!File.Exists(path)) return null;

            string json = await File.ReadAllTextAsync(path);
            var allMeditations = JsonSerializer.Deserialize<List<LocalMeditation>>(json);

            
            return allMeditations?.FirstOrDefault(m => m.Date == day);
        }
        catch { return null; }
    }

    private async Task<bool> DownloadAllMeditationsForMystery(string mystery)
    {
        try
        {
          
            var list = await _meditationService.GetAllMeditationsForMystery(mystery);

            if (list != null && list.Any())
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, $"meditations_{mystery.GetHashCode()}.json");
                string json = JsonSerializer.Serialize(list);
                await File.WriteAllTextAsync(path, json);
                return true;
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Błąd pobierania całości: {ex.Message}"); }
        return false;
    }
}
