using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace MauiApp1;


public partial class FullMeditationPage : ContentPage, IQueryAttributable
{
    public int date;
    private string _meditationText;
    private bool _isBusy = false;
    private readonly NotificationsService _notificationService;
    public MeditationsService _meditationService;
    public AuthService _authService;
    public FullMeditationPage(NotificationsService notificationsService, MeditationsService meditationService, AuthService authService)
    {
        InitializeComponent();
        _notificationService = notificationsService;
        _meditationService = meditationService;
        _authService = authService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("MeditationContent"))
        {

            _meditationText = query["MeditationContent"] as string;


            if (MeditationWebView != null)
            {
                LoadHtmlToWebView(_meditationText);
            }
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isBusy = true;

        date = Preferences.Default.Get("LastDate", 0);
        string savedGroup = Preferences.Default.Get("LastGroup", "Radosne");
        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
        _isBusy = false;
       // UpdateUI();
        UpdateDate();
    }

    private async void UpdateDate()
    {
        if (_isBusy) return;
        try
        {
            DateLabel.Text = "Dzień " + date;
            

            string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            MysteryLabel.Text = selectedMystery;
            if (string.IsNullOrEmpty(selectedMystery)) return;

            LoadHtmlToWebView("<p style='text-align: center; font-style: italic;'>Ładowanie ....</p>");

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
            LoadHtmlToWebView("<p style='text-align: center; color: red;'>Błąd połączenia</p>");
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
    private void ApplyMeditationData(LocalMeditation data)
    {
        string rawContent = data?.Content ?? "Brak rozważania";
        LoadHtmlToWebView(rawContent);
    }
    private void LoadHtmlToWebView(string contentHtml)
    {
        if (MeditationWebView == null) return;

        bool isDark = Preferences.Get("app_main_theme", false);

        string bgColor = isDark ? "#080808" : "#FAFAFA";
        string textColor = isDark ? "#F1F1F1" : "#1F1F1F";
        string headingColor = isDark ? "#F1F1F1" : "#1F1F1F";

        string fullHtmlPage = $@"
<!DOCTYPE html>  
<html lang='pl'>  
<head>      
    <meta charset='utf-8'>      
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>      
    <style>          
        body {{              
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Arial, sans-serif;              
            font-size: 16px;              
            line-height: 1.6;              
            color: {textColor};              
            padding: 8px;              
            margin: 0;              
            background-color: {bgColor};                              
            text-align: justify;              
            text-justify: inter-word;                              
            -webkit-hyphens: auto;              
            -moz-hyphens: auto;              
            -ms-hyphens: auto;              
            hyphens: auto;
        }}          
        p {{              
            text-align: justify;              
            text-justify: inter-word;              
            margin-top: 0;
            margin-bottom: 12px;
        }}          
        h1, h2, h3 {{               
            color: {headingColor};               
            margin-top: 18px; 
            margin-bottom: 8px;              
            text-align: left;              
            -webkit-hyphens: none !important;              
            -ms-hyphens: none !important;              
            hyphens: none !important;               
            word-break: keep-all; 
        }}
        
        /* Mapowanie czcionek systemowych z edytora Quill */
        .ql-font-arial {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Arial, sans-serif; }}
        .ql-font-georgia {{ font-family: Georgia, 'Times New Roman', serif; }}
        .ql-font-times {{ font-family: 'Times New Roman', Times, serif; }}
        .ql-font-courier {{ font-family: 'Courier New', Courier, monospace; }}
        .ql-font-trebuchet {{ font-family: 'Trebuchet MS', 'Lucida Sans Unicode', sans-serif; }}
        .ql-font-verdana {{ font-family: Verdana, Geneva, sans-serif; }}

        /* Wyrównania tekstu Quill */
        .ql-align-center {{ text-align: center; }}
        .ql-align-right {{ text-align: right; }}
        .ql-align-justify {{ text-align: justify; }}
    </style>  
</head>  
<body>      
    {contentHtml}  
</body>  
</html>";

        MeditationWebView.Source = new HtmlWebViewSource
        {
            Html = fullHtmlPage
        };
    }
    private async void CompletedTapped(object sender, TappedEventArgs e)
    {
        Color? color = Complete.BackgroundColor;
        float newAlpha = color.Alpha < 1f ? 1f : 0.5f;
        Complete.BackgroundColor = color.WithAlpha(newAlpha);
        string todayKey = DateTime.Today.ToString("yyyy-MM-dd");
        Preferences.Default.Set("LastCompleteDate", date);
     //   Preferences.Default.Set($"Done_{todayKey}", true);
      //  var handler = new JwtSecurityTokenHandler();
      //  var jsonToken = handler.ReadJwtToken(_authService.Token);
    //    var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        //if (int.TryParse(IdClaim?.Value, out int id))
        //{
        //    await _meditationService.RecordPrayerAsync(id, DateTime.Today);
        //}
        
        //await _notificationService.ScheduleWeeklyReminders();
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
        if (--date < 0) date = 31;
        UpdateDate();
    }

    private async void NextTapped(object sender, EventArgs e)
    {
        if (++date > 31) date = 0;
        UpdateDate();
    }
}