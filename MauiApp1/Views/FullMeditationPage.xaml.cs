using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.Json;

namespace MauiApp1;

public partial class FullMeditationPage : ContentPage, IQueryAttributable
{
    public int date;
    private string? _meditationText;
    private readonly NotificationsService _notificationService;
    private readonly MeditationsService _meditationService;
    private readonly AuthService _authService;

    public FullMeditationPage(NotificationsService notificationsService, MeditationsService meditationService, AuthService authService)
    {
        InitializeComponent();

        _notificationService = notificationsService;
        _meditationService = meditationService;
        _authService = authService;

        MeditationWebView.Navigated += MeditationWebView_Navigated;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("MeditationContent", out object? value) &&
            value is string content && !string.IsNullOrWhiteSpace(content))
        {
            _meditationText = content;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        date = Preferences.Default.Get("LastDate", 0);

        string savedGroup = Preferences.Default.Get("LastGroup", "Radosne");

        string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");

        SetLoadingState(true);

        _ = UpdateDateAsync();
    }

    private async Task UpdateDateAsync()
    {
        DateLabel.Text = $"Dzień {date}";

        string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");

        MysteryLabel.Text = selectedMystery;

        if (string.IsNullOrWhiteSpace(selectedMystery))
        {
            ShowErrorInWebView("Nie wybrano tajemnicy.");
            return;
        }

        SetLoadingState(true);

        await Task.Yield();

        LocalMeditation? localData = await GetMeditationFromLocalFile(date, selectedMystery);

        if (localData != null)
        {
            ApplyMeditationData(localData);
            return;
        }

        bool autoDownload = Preferences.Default.Get(
            "AutoDownloadMeditations",
            false);

        if (autoDownload)
        {
            bool downloaded = await DownloadAllMeditationsForMystery(selectedMystery);

            if (downloaded)
            {
                LocalMeditation? freshLocalData =
                    await GetMeditationFromLocalFile(date, selectedMystery);

                if (freshLocalData != null)
                {
                    ApplyMeditationData(freshLocalData);
                    return;
                }
            }
        }

        LocalMeditation? data = await _meditationService.GetMeditationData(date, selectedMystery);

        ApplyMeditationData(data);
    }

    private void ApplyMeditationData(LocalMeditation? data)
    {
        string rawContent = data?.Content ?? "Brak rozważania.";

        LoadHtmlToWebView(rawContent);
    }

    private void ShowErrorInWebView(string message)
    {
        string html = $@"
<p style='
    text-align: center;
    color: #C62828;
    padding: 24px 8px;
    font-family: Arial, sans-serif;
'>
    {message}
</p>";

        LoadHtmlToWebView(html);
    }

    private void SetLoadingState(bool isLoading)
    {
        LoadingPanel.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;

        if (isLoading)
        {
            MeditationWebView.IsVisible = false;
        }
    }

    private void LoadHtmlToWebView(string contentHtml)
    {
        if (MeditationWebView == null)
        {
            return;
        }

        bool isDark = Preferences.Default.Get("app_main_theme", false);

        string bgColor = isDark ? "#080808" : "#FAFAFA";
        string textColor = isDark ? "#F1F1F1" : "#1F1F1F";
        string headingColor = isDark ? "#F1F1F1" : "#1F1F1F";

        string fullHtmlPage = $@"
<!DOCTYPE html>
<html lang='pl'>
<head>
    <meta charset='utf-8'>

    <meta
        name='viewport'
        content='width=device-width, initial-scale=1.0,
                 maximum-scale=1.0, user-scalable=no'>

    <meta name='color-scheme' content='{(isDark ? "dark" : "light")}'>

    <style>
        html, body {{
            width: 100%;
            min-height: 100%;
            padding: 0;
            margin: 0;
            background-color: {bgColor};
            color: {textColor};
        }}

        body {{
            box-sizing: border-box;
            padding: 8px;

            font-family: -apple-system, BlinkMacSystemFont,
                         'Segoe UI', Roboto, Arial, sans-serif;

            font-size: 16px;
            line-height: 1.6;

            text-align: justify;
            text-justify: inter-word;

            -webkit-hyphens: auto;
            -moz-hyphens: auto;
            -ms-hyphens: auto;
            hyphens: auto;
        }}

        p {{
            margin-top: 0;
            margin-bottom: 12px;

            text-align: justify;
            text-justify: inter-word;
        }}

        h1, h2, h3 {{
            margin-top: 18px;
            margin-bottom: 8px;

            color: {headingColor};

            text-align: left;

            -webkit-hyphens: none !important;
            -ms-hyphens: none !important;
            hyphens: none !important;

            word-break: keep-all;
        }}

        /* Czcionki używane przez Quill */
        .ql-font-arial {{
            font-family: -apple-system, BlinkMacSystemFont,
                         'Segoe UI', Roboto, Arial, sans-serif;
        }}

        .ql-font-georgia {{
            font-family: Georgia, 'Times New Roman', serif;
        }}

        .ql-font-times {{
            font-family: 'Times New Roman', Times, serif;
        }}

        .ql-font-courier {{
            font-family: 'Courier New', Courier, monospace;
        }}

        .ql-font-trebuchet {{
            font-family: 'Trebuchet MS',
                         'Lucida Sans Unicode', sans-serif;
        }}

        .ql-font-verdana {{
            font-family: Verdana, Geneva, sans-serif;
        }}

        /* Wyrównania tekstu Quill */
        .ql-align-center {{
            text-align: center;
        }}

        .ql-align-right {{
            text-align: right;
        }}

        .ql-align-justify {{
            text-align: justify;
        }}
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

    private void MeditationWebView_Navigated(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Success)
        {
            MeditationWebView.IsVisible = true;

            LoadingIndicator.IsRunning = false;
            LoadingPanel.IsVisible = false;
        }
        else
        {
            MeditationWebView.IsVisible = true;

            LoadingIndicator.IsRunning = false;
            LoadingPanel.IsVisible = false;
        }
    }

    private void CompletedTapped(object sender, TappedEventArgs e)
    {
        Color currentColor = Complete.BackgroundColor ?? Colors.Transparent;

        float newAlpha = currentColor.Alpha < 1f
            ? 1f
            : 0.5f;

        Complete.BackgroundColor = currentColor.WithAlpha(newAlpha);

        Preferences.Default.Set("LastCompleteDate", date);
    }

    private async Task<LocalMeditation?> GetMeditationFromLocalFile(int day, string mystery)
    {
        try
        {
            string path = GetFileName(mystery);
            bool fileExists = File.Exists(path);

            if (!fileExists)
            {
                return null;
            }

            string json = await File.ReadAllTextAsync(path);

            List<LocalMeditation>? allMeditations =
                JsonSerializer.Deserialize<List<LocalMeditation>>(json);

            LocalMeditation? result = allMeditations?
                .FirstOrDefault(meditation => meditation.Date == day);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<bool> DownloadAllMeditationsForMystery(string mystery)
    {
        try
        {
            List<LocalMeditation>? list =
                await _meditationService.GetAllMeditationsForMystery(mystery);

            if (list == null || list.Count == 0)
            {
                return false;
            }

            string json = JsonSerializer.Serialize(list);
            string path = GetFileName(mystery);

            await File.WriteAllTextAsync(path, json);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private string GetFileName(string mystery)
    {
        string safeName = mystery
            .Replace(" ", "_")
            .Substring(0, Math.Min(mystery.Length, 20));

        string path = Path.Combine(
            FileSystem.AppDataDirectory,
            $"meditations_{safeName}.json");

        return path;
    }

    private async void PreviousTapped(object sender, EventArgs e)
    {
        if (--date < 0) date = 31;
        await UpdateDateAsync();
    }

    private async void NextTapped(object sender, EventArgs e)
    {
        if (++date > 31) date = 0;
        await UpdateDateAsync();
    }
}