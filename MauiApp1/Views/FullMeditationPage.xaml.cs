using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.Json;

namespace MauiApp1;


public partial class FullMeditationPage : ContentPage, IQueryAttributable
{
    public int date;
    private string _meditationText;
    private bool _isBusy = false;
    private readonly NotificationsService _notificationService;
    public MeditationsService _meditationService;
    public FullMeditationPage(NotificationsService notificationsService, MeditationsService meditationService)
    {
        InitializeComponent();
        _notificationService = notificationsService;
        _meditationService = meditationService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("MeditationContent"))
        {

            _meditationText = query["MeditationContent"] as string;


            if (FullMeditation != null)
            {
                FullMeditation.Text = _meditationText;
            }
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isBusy = true;

        date = Preferences.Default.Get("LastDate", 1);
        string savedGroup = Preferences.Default.Get("LastGroup", "Radosne");
        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
        _isBusy = false;
       // UpdateUI();
        UpdateDate();
    }

    /*  private void UpdateUI()
      {

          if (FullMeditation != null && !string.IsNullOrEmpty(_meditationText))
          {
              int date = Preferences.Default.Get("LastDate", 1);
              DateLabel.Text = "Dzień " + date.ToString();
              FullMeditation.Text = _meditationText;
          }
     }*/
    private async void UpdateDate()
    {
        if (_isBusy) return;
        try
        {
            DateLabel.Text = "dzień " + date;
            Preferences.Default.Set("LastDate", date);

            string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            if (string.IsNullOrEmpty(selectedMystery)) return;

           FullMeditation.Text = "Ładowanie ....";


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
            FullMeditation.Text = "Błąd połączenia";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
    private void ApplyMeditationData(LocalMeditation data)
    {
        FullMeditation.Text = data?.Content ?? "Brak rozważania";
    }
    private async void CompletedTapped(object sender, TappedEventArgs e)
    {
        Color? color = Complete.BackgroundColor;
        float newAlpha = color.Alpha < 1f ? 1f : 0.5f;
        Complete.BackgroundColor = color.WithAlpha(newAlpha);
        string todayKey = DateTime.Today.ToString("yyyy-MM-dd");
        Preferences.Default.Set($"Done_{todayKey}", true);

        
        await _notificationService.ScheduleWeeklyReminders();
    }

    private async void BackTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async Task<LocalMeditation> GetMeditationFromLocalFile(int day, string mystery)
    {
        try
        {
            string path = GetFileName(mystery);
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
                string path = GetFileName(mystery);
                string json = JsonSerializer.Serialize(list);
                await File.WriteAllTextAsync(path, json);
                return true;
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Błąd pobierania całości: {ex.Message}"); }
        return false;
    }

    private string GetFileName(string mystery)
    {

        string safeName = mystery.Replace(" ", "_").Substring(0, Math.Min(mystery.Length, 20));
        return Path.Combine(FileSystem.AppDataDirectory, $"meditations_{safeName}.json");
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
}