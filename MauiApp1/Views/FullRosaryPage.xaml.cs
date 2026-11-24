using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.Json;
using CommunityToolkit.Maui.Views;

namespace MauiApp1.Views
{
    public partial class FullRosaryPage : ContentPage
    {
        public int date = 1;
        private bool _isBusy;
        private string _part = string.Empty;
        private RosaryPartProgress _progress = new();
        private readonly RosaryProgressStore _progressStore = new(
            Path.Combine(FileSystem.AppDataDirectory, "rosary-progress.json"));
        private readonly NotificationsService _notificationService;
        public MeditationsService _meditationService;
        public AuthService _authService;

        private static readonly Dictionary<string, List<string>> _mysteriesMap = new()
        {
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

        public FullRosaryPage(NotificationsService notificationsService, MeditationsService meditationService, AuthService authService)
        {
            InitializeComponent();
            _notificationService = notificationsService;
            _meditationService = meditationService;
            _authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_isBusy) return;
            _isBusy = true;
            try
            {
                _part = GetDefaultRosaryPartForToday();
                _progress = await _progressStore.LoadAsync(_part);
                date = _progress.Day;
                RenderMysteries();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                _part = string.Empty;
                MysteriesContainer.Children.Clear();
                await DisplayAlertAsync("Postęp różańca", "Nie udało się odczytać zapisanego postępu. Spróbuj ponownie otworzyć zakładkę.", "OK");
            }
            finally { _isBusy = false; }
        }

        private string GetDefaultRosaryPartForToday()
        {
            return DateTime.Today.DayOfWeek switch
            {
                DayOfWeek.Monday => "Radosne",
                DayOfWeek.Tuesday => "Bolesne",
                DayOfWeek.Wednesday => "Chwalebne",
                DayOfWeek.Thursday => "Światła",
                DayOfWeek.Friday => "Bolesne",
                DayOfWeek.Saturday => "Radosne",
                DayOfWeek.Sunday => "Chwalebne",
                _ => "Radosne"
            };
        }

        private void RenderMysteries()
        {
            DateLabel.Text = $"Różaniec ({_part}) — dzień {date}";
            MysteriesContainer.Children.Clear();
            for (int i = 0; i < _mysteriesMap[_part].Count; i++)
                AddMystery(i, _mysteriesMap[_part][i], date, _part);
        }

        private void AddMystery(int index, string mystery, int day, string part)
        {
            bool completed = _progress.IsComplete(day, index);
            var header = new Label
            {
                Text = $"{index + 1}. {mystery}" + (completed ? " ✓ Ukończono" : " ▾"),
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(4, 8)
            };
            header.SetDynamicResource(Label.TextColorProperty, "Tertiary");
            var webView = new WebView { HeightRequest = 420 };
            var complete = new Button
            {
                Text = completed ? "Ukończono ✓" : "Ukończono",
                IsEnabled = false,
                FontSize = 18,
                MinimumHeightRequest = 48,
                CornerRadius = 20
            };
            complete.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
            complete.SetDynamicResource(Button.TextColorProperty, "Tertiary");
            var content = new VerticalStackLayout { Spacing = 4 };
            content.Children.Add(webView);
            content.Children.Add(complete);
            var expander = new Expander { Header = header, Content = content };
            var border = new Border { Content = expander, Margin = 0, Padding = 0 };
            border.SetDynamicResource(StyleProperty, "MenuOption");
            MysteriesContainer.Children.Add(border);

            bool loading = false;
            bool loaded = false;
            expander.ExpandedChanged += async (_, _) =>
            {
                if (!expander.IsExpanded || loading || loaded) return;
                loading = true;
                try
                {
                    LoadHtmlToWebView(webView, "<p>Ładowanie rozważania…</p>");
                    var meditation = await GetMeditationFromLocalFile(day, mystery);
                    if (meditation == null && Preferences.Default.Get("AutoDownloadMeditations", false))
                    {
                        await DownloadAllMeditationsForMystery(mystery);
                        meditation = await GetMeditationFromLocalFile(day, mystery);
                    }
                    meditation ??= await _meditationService.GetMeditationData(day, mystery);
                    loaded = !string.IsNullOrWhiteSpace(meditation?.Content);
                    LoadHtmlToWebView(webView, loaded ? meditation!.Content :
                        "<p>Brak rozważania. Sprawdź połączenie, zwiń i ponownie rozwiń tajemnicę, aby spróbować jeszcze raz.</p>");
                    complete.IsEnabled = loaded && !completed;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    LoadHtmlToWebView(webView, "<p>Nie udało się pobrać rozważania. Zwiń i ponownie rozwiń tajemnicę, aby spróbować jeszcze raz.</p>");
                }
                finally { loading = false; }
            };
            complete.Clicked += async (_, _) =>
            {
                if (_isBusy || completed) return;
                _isBusy = true;
                complete.IsEnabled = false;
                try
                {
                    _progress = await _progressStore.CompleteAsync(part, day, index);
                    completed = true;
                    complete.Text = "Ukończono ✓";
                    header.Text = $"{index + 1}. {mystery} ✓ Ukończono";
                    expander.IsExpanded = false;
                    if (_progress.Completed.TryGetValue(day, out var done) && done.Count == 5)
                        DateLabel.Text = $"Różaniec ({part}) — dzień {day} ukończony";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    complete.IsEnabled = true;
                    await DisplayAlertAsync("Zapis postępu", "Nie udało się zapisać ukończenia. Spróbuj ponownie.", "OK");
                }
                finally { _isBusy = false; }
            };
        }

        private void LoadHtmlToWebView(WebView meditationWebView, string contentHtml)
        {
            if (meditationWebView == null) return;

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

            meditationWebView.Source = new HtmlWebViewSource
            {
                Html = fullHtmlPage
            };
        }


        private void PreviousTapped(object sender, EventArgs e)
        {
            if (_isBusy || string.IsNullOrEmpty(_part)) return;
            if (--date < 1) date = 31;
            RenderMysteries();
        }

        private void NextTapped(object sender, EventArgs e)
        {
            if (_isBusy || string.IsNullOrEmpty(_part)) return;
            if (++date > 31) date = 1;
            RenderMysteries();
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania całości: {ex.Message}");
            }
            return false;
        }

        private string GetFileName(string mystery)
        {
            string safeName = mystery.Replace(" ", "_").Substring(0, Math.Min(mystery.Length, 20));
            return Path.Combine(FileSystem.AppDataDirectory, $"meditations_{safeName}.json");
        }
    }
}
