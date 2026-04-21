using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public int date;
        public MeditationsService _meditationService;
        public AuthService _authService;
        public RosaryService _rosaryService;
        private static readonly Dictionary<string, List<string>> _itemsMap = new() {
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
        private string _selectedPart = null;
        public MainPage(MeditationsService meditationService, AuthService authService, RosaryService rosaryService)
        {
            InitializeComponent();
            _meditationService = meditationService;
            _authService = authService;
            _rosaryService = rosaryService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _selectedPart = null;
            StarterAnimation();
            
            bool hasToken = await _authService.CheckAndSetTokenAsync();
            if (hasToken)
            {
                RosaryTile.IsVisible = true;
            }
            else
            {
                RosaryTile.IsVisible = false;
            }
            await UpdateMeditation();

            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                btn.IsVisible = false;
                btn.TranslationX = btn.TranslationY = 0;
            }
        }

        private async void StarterAnimation()
        {

            foreach (var btn in new[] { Radosne, Swiatla, Bolesne, Chwalebne })
            {
                btn.TranslationX = btn.TranslationY = 0;
                btn.Scale = 1;
            }

            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(1000), () =>
            {
                var img = CenterImage.ScaleToAsync(2, 750, Easing.SinInOut);

                var b1 = Radosne.TranslateToAsync(-100, -100, 1000, Easing.SinInOut);
                var b1Scale = Radosne.ScaleToAsync(2, 1000, Easing.SinInOut);

                var b2 = Swiatla.TranslateToAsync(-100, +100, 1000, Easing.SinInOut);
                var b2Scale = Swiatla.ScaleToAsync(2, 1000, Easing.SinInOut);

                var b3 = Bolesne.TranslateToAsync(+100, -100, 1000, Easing.SinInOut);
                var b3Scale = Bolesne.ScaleToAsync(2, 1000, Easing.SinInOut);

                var b4 = Chwalebne.TranslateToAsync(+100, +100, 1000, Easing.SinInOut);
                var b4Scale = Chwalebne.ScaleToAsync(2, 1000, Easing.SinInOut);

                Task.WhenAll(b1, b1Scale, b2, b2Scale, b3, b3Scale, b4, b4Scale);
            });
        }

        private async void RosaryPart_Tapped(object sender, TappedEventArgs e)
        {
            Border s = sender as Border;
            string partName = e.Parameter.ToString();

            if (_selectedPart == partName)
            {
                await CloseMystryAnimation(s);
                switch (partName)
            {
                case "Radosne":
                    s.TranslateToAsync(-100, -100, 1000, Easing.SinInOut);
                    break;
                case "Światła":
                    s.TranslateToAsync(-100, +100, 1000, Easing.SinInOut);
                    break;
                case "Bolesne":
                    s.TranslateToAsync(+100, -100, 1000, Easing.SinInOut);
                    break;
                case "Chwalebne":
                    s.TranslateToAsync(+100, +100, 1000, Easing.SinInOut);
                    break;
            }
                _selectedPart = null;
            }
            else if (_selectedPart!=null)
            {
                return;
            }
            else
            {
                s.TranslateToAsync(0, 0, 1000, Easing.SinInOut);
                _selectedPart = partName;

                await ShowMysteryAnimation(s);
            }
        }

        private async Task ShowMysteryAnimation(Border s)
        {
            var animationTasks = new List<Task>();
            double radius = s.Height / 2 + 50;
            double center = s.Height / 2;
            double angleOffset = -Math.PI / 10;
            int i = 0;
            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                btn.IsVisible = true;
                btn.Opacity = 0;
                double btnSize = btn.Height;
                double angle = i * 2 * Math.PI / 5 + angleOffset;
                double tx = center + radius * Math.Cos(angle) - btnSize / 2;
                double ty = center + radius * Math.Sin(angle) - btnSize / 2;
                var btnTranslate = btn.TranslateToAsync(tx, ty, 1000, Easing.SinInOut);
                var btnFade = btn.FadeToAsync(1, 1000, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
                i++;
            }
            await Task.WhenAll(animationTasks);
        }

        private async Task CloseMystryAnimation(Border s)
        {
            var animationTasks = new List<Task>();
            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                double btnSize = btn.Height;
                var btnTranslate = btn.TranslateToAsync(0, 0, 1000, Easing.SinInOut);
                var btnFade = btn.FadeToAsync(0, 1000, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
            }

            await Task.WhenAll(animationTasks);

            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                btn.IsVisible = false;
            }
        }

        private async void MyRosaryGroup_Tapped(object sender, TappedEventArgs e)
        {
            if (string.IsNullOrEmpty(_authService.Token)) return;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(_authService.Token);

            var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
            int userRole = int.Parse(roleClaim?.Value ?? "5");
            if (userRole==0 || userRole == 1)
            {
                await Shell.Current.GoToAsync("MyRosariesList");
            }
            else
            {
               
                var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
                if (IdClaim != null && int.TryParse(IdClaim.Value, out int Id))
                {
                    int rosaryId;
                    List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(Id);
                    if (rosaryInfos != null && rosaryInfos.Count == 1)
                    {
                        rosaryId = rosaryInfos[0].Id;
                        var navigationParameter = new Dictionary<string, object>{{ "RosaryId", rosaryId.ToString() }, { "UserRole", userRole }};
                        await Shell.Current.GoToAsync("MyRosaryGroup", navigationParameter);
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("SelectParish", new Dictionary<string, object> { { "UserId", Id} });
                    }
                }
            }
        }

        private async void RosaryMeditations_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("RosaryMeditations");
        }

        private async void Meditation_Tapped(object sender, TappedEventArgs e)
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

        private async Task UpdateMeditation()
        {
            try
            {
               
                date=Preferences.Default.Get("LastDate", 1);
                
                string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
                if (string.IsNullOrEmpty(selectedMystery)) return;

                MeditationLabel.Text = "Ładowanie ....";


                var localData = await GetMeditationFromLocalFile(this.date, selectedMystery);

                if (localData != null)
                {
                    DateLabel.Text = "Dzień " + this.date;
                    MysteryLabel.Text = selectedMystery;
                    MeditationLabel.Text = localData?.Content ?? "Brak rozważania";
                  
                    return;
                }

                var data = await _meditationService.GetMeditationData(this.date, selectedMystery);
                DateLabel.Text = "Dzień " + this.date;
                MysteryLabel.Text = selectedMystery;
                MeditationLabel.Text = data?.Content ?? "Brak rozważania";
            }
            catch (Exception ex)
            {
                MeditationLabel.Text = "Błąd połączenia";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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

        private string GetFileName(string mystery)
        {
            string safeName = mystery.Replace(" ", "_").Substring(0, Math.Min(mystery.Length, 20));
            return Path.Combine(FileSystem.AppDataDirectory, $"meditations_{safeName}.json");
        }
    }
}
