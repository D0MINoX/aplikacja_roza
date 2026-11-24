using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MauiApp1;

public partial class FullMeditationPage : ContentPage, IQueryAttributable
{
    private static readonly Lazy<Task<Dictionary<string, Dictionary<int, string>>>> SectionTitles =
        new(LoadSectionTitlesAsync);
    private int _dateUpdateVersion;

    public int date;
    private string _meditationText;
    private bool _isBusy = false;
    private readonly NotificationsService _notificationService;
    public MeditationsService _meditationService;
    public AuthService _authService;

    private string _plainText = "";
    private CancellationTokenSource? _ttsCts;
    private bool _isSpeaking;

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
            _plainText = HtmlToPlainText(_meditationText);

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

        StopSpeaking();
        int updateVersion = ++_dateUpdateVersion;
        int selectedDay = date;
        Complete.Opacity = 1;
        SectionTitleLabel.IsVisible = false;

        try
        {
            string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            MysteryLabel.Text = selectedMystery;
            if (string.IsNullOrEmpty(selectedMystery)) return;

            LoadHtmlToWebView("<p style='text-align: center; font-style: italic;'>Ładowanie ....</p>");

            string? sectionTitle = await GetSectionTitleAsync(selectedMystery, selectedDay);
            if (updateVersion != _dateUpdateVersion) return;

            SectionTitleLabel.Text = sectionTitle;
            SectionTitleLabel.IsVisible = !string.IsNullOrWhiteSpace(sectionTitle);

            var data = await GetMeditationFromLocalFile(selectedDay, selectedMystery);
            if (data == null &&
                Preferences.Default.Get("AutoDownloadMeditations", false) &&
                await DownloadAllMeditationsForMystery(selectedMystery))
            {
                data = await GetMeditationFromLocalFile(selectedDay, selectedMystery);
            }

            data ??= await _meditationService.GetMeditationData(selectedDay, selectedMystery);
            if (updateVersion == _dateUpdateVersion)
                ApplyMeditationData(data);
        }
        catch (Exception ex)
        {
            if (updateVersion == _dateUpdateVersion)
                LoadHtmlToWebView("<p style='text-align: center; color: red;'>Błąd połączenia</p>");

            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private static async Task<string?> GetSectionTitleAsync(string mystery, int day)
    {
        int sectionStart = day switch
        {
            >= 1 and <= 4 => 1,
            >= 5 and <= 11 => 5,
            >= 12 and <= 18 => 12,
            >= 19 and <= 25 => 19,
            >= 26 and <= 30 => 26,
            _ => 0
        };
        if (sectionStart == 0) return null;

        var titles = await SectionTitles.Value;
        return titles.TryGetValue(mystery, out var sections) &&
               sections.TryGetValue(sectionStart, out var title)
            ? title
            : null;
    }

    private static async Task<Dictionary<string, Dictionary<int, string>>> LoadSectionTitlesAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("meditation-section-titles.json");
            var titles = await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<int, string>>>(stream);
            if (titles != null)
                return new(titles, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        return new(StringComparer.OrdinalIgnoreCase);
    }

    private void ApplyMeditationData(LocalMeditation data)
    {
        string rawContent = data?.Content ?? "Brak rozważania";
        _plainText = HtmlToPlainText(rawContent);
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
    <meta http-equiv='Content-Security-Policy' content=""script-src 'none'; object-src 'none'; frame-src 'none'; connect-src 'none'; base-uri 'none'; form-action 'none'"">
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Arial, sans-serif;
            font-size: 16px;
            line-height: 1.6;
            color: {textColor};
            padding: 4px;
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

    private void CompletedTapped(object sender, TappedEventArgs e)
    {
        string mystery = Preferences.Default.Get("LastMystery", string.Empty);
        RosaryCycle.Complete(mystery, date, DateTime.Today);
        Complete.Opacity = 0.5;
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

    private static string HtmlToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return "";

        // nagłówki: kropka, żeby lektor zrobił pauzę
        string t = Regex.Replace(html, @"</h[1-6]\s*>", ".\n", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, @"</p\s*>|<br\s*/?>|</li\s*>", "\n", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, @"<[^>]+>", "");
        t = WebUtility.HtmlDecode(t).Replace('\u00A0', ' ');
        t = Regex.Replace(t, @"[ \t]+", " ");
        t = Regex.Replace(t, @"\n\s*\n+", "\n");
        return t.Trim();
    }

    // Android ogranicza długość jednego Speak (~4000 znaków), więc dzielimy po akapitach
    private static IEnumerable<string> SplitForSpeech(string text, int maxLen = 3000)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var para in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            if (sb.Length + para.Length + 1 > maxLen && sb.Length > 0)
            {
                yield return sb.ToString();
                sb.Clear();
            }
            if (para.Length > maxLen)
            {
                foreach (var s in Regex.Split(para, @"(?<=[\.\!\?])\s+"))
                {
                    if (sb.Length + s.Length + 1 > maxLen && sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }
                    sb.Append(s).Append(' ');
                }
            }
            else sb.Append(para).Append('\n');
        }
        if (sb.Length > 0) yield return sb.ToString();
    }

    private async void ReadTapped(object sender, TappedEventArgs e)
    {
        if (_isSpeaking)
        {
            StopSpeaking();
            return;
        }
        if (string.IsNullOrWhiteSpace(_plainText)) return;

        _ttsCts = new CancellationTokenSource();
        var token = _ttsCts.Token;
        _isSpeaking = true;
        ReadLabel.Text = "⏹";

        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var pl = locales.FirstOrDefault(l => l.Language.StartsWith("pl", StringComparison.OrdinalIgnoreCase));
            var options = new SpeechOptions { Locale = pl };

            foreach (var chunk in SplitForSpeech(_plainText))
            {
                token.ThrowIfCancellationRequested();
                await TextToSpeech.Default.SpeakAsync(chunk, options, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TTS error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się odczytać tekstu.", "OK");
        }
        finally
        {
            _isSpeaking = false;
            ReadLabel.Text = "🔊";
        }
    }

    private void StopSpeaking()
    {
        if (_ttsCts is { IsCancellationRequested: false })
            _ttsCts.Cancel();
        _ttsCts?.Dispose();
        _ttsCts = null;
        _isSpeaking = false;
        ReadLabel.Text = "🔊";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopSpeaking();
    }
}