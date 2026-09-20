using MauiApp1.Models;
using MauiApp1.Services;
using System.Text.Json;

namespace MauiApp1.Views
{
    public partial class FullRosaryPage : ContentPage
    {
        public int date = 1;
        private bool _isBusy = false;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            date = Preferences.Default.Get("LastDate", 1);
            if (date <= 0) date = 1;

            LoadFullRosary();
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

        private async void LoadFullRosary()
        {
            if (_isBusy) return;
            _isBusy = true;

            try
            {
                // Odczytujemy zapisy lub wyznaczamy wg dnia tygodnia
                string fallbackPart = GetDefaultRosaryPartForToday();
                string savedGroup = Preferences.Default.Get("LastGroup", fallbackPart);

                DateLabel.Text = $"Różaniec ({savedGroup}) - Dzień {date}";

                LoadHtmlToWebView("<p style='text-align: center; font-style: italic;'>Ładowanie rozważań całego różańca...</p>");

                if (!_mysteriesMap.ContainsKey(savedGroup))
                {
                    savedGroup = fallbackPart;
                }

                List<string> mysteries = _mysteriesMap[savedGroup];
                var fullContentBuilder = new System.Text.StringBuilder();

                bool autoDownload = Preferences.Default.Get("AutoDownloadMeditations", false);

                for (int i = 0; i < mysteries.Count; i++)
                {
                    string mysteryName = mysteries[i];

                    fullContentBuilder.AppendLine($"<h2 style='color: #8B0000; border-bottom: 2px solid #8B0000; padding-bottom: 4px; margin-top: 25px;'>Tajemnica {i + 1}: {mysteryName}</h2>");

                    LocalMeditation meditation = await GetMeditationFromLocalFile(this.date, mysteryName);

                    if (meditation == null && autoDownload)
                    {
                        await DownloadAllMeditationsForMystery(mysteryName);
                        meditation = await GetMeditationFromLocalFile(this.date, mysteryName);
                    }

                    if (meditation == null)
                    {
                        var data = await _meditationService.GetMeditationData(this.date, mysteryName);
                        if (data != null)
                        {
                            meditation = data;
                        }
                    }

                    string text = meditation?.Content ?? "<p><i>Brak rozważania dla tej tajemnicy.</i></p>";
                    fullContentBuilder.AppendLine($"<div>{text}</div><hr style='border: 0; height: 1px; background: #ccc; margin: 20px 0;'/>");
                }

                LoadHtmlToWebView(fullContentBuilder.ToString());
            }
            catch (Exception ex)
            {
                LoadHtmlToWebView("<p style='text-align: center; color: red;'>Błąd podczas ładowania rozważań.</p>");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void LoadHtmlToWebView(string contentHtml)
        {
            if (MeditationWebView == null) return;

            string fullHtmlPage = $@"
<!DOCTYPE html>  
<html lang='pl'>  
<head>      
    <meta charset='utf-8'>      
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>      
    <style>          
        body {{              
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;              
            font-size: 16px;              
            line-height: 1.6;              
            color: #333333;              
            padding: 8px;              
            margin: 0;              
            background-color: transparent;                              
            text-align: justify;              
            text-justify: inter-word;
            white-space: pre-line; 
        }}          
        p {{              
            text-align: justify;              
            margin-bottom: 12px;                              
            white-space: pre-line; 
        }}          
        h2 {{               
            font-size: 18px;
            font-weight: bold;
            margin-top: 20px;               
            text-align: left;              
            word-break: keep-all; 
            white-space: normal; 
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

        private async void CompletedTapped(object sender, TappedEventArgs e)
        {
            Color? color = Complete.BackgroundColor;
            float newAlpha = color.Alpha < 1f ? 1f : 0.5f;
            Complete.BackgroundColor = color.WithAlpha(newAlpha);

            Preferences.Default.Set("LastCompleteDate", date);
        }

        private async void PreviousTapped(object sender, EventArgs e)
        {
            if (--date < 1) date = 31;
            LoadFullRosary();
        }

        private async void NextTapped(object sender, EventArgs e)
        {
            if (++date > 31) date = 1;
            LoadFullRosary();
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