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
        private static readonly Dictionary<string, List<string>> _imagesMap = new(){
            ["Radosne"] = new()
        {
            "theme.png",
            "test.png",
            "test.png",
            "test.png",
            "test.png"
        },
            ["Światła"] = new()
        {
            "flower.png",
            "test.png",
            "test.png",
            "test.png",
            "test.png"
        },
            ["Bolesne"] = new()
        {
            "test.png",
            "test.png",
            "test.png",
            "test.png",
            "test.png"
        },
            ["Chwalebne"] = new()
        {
            "test.png",
            "test.png",
            "test.png",
            "test.png",
            "eg_meditation.png"
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
                await CloseMystryAnimation();
                var scale = s.ScaleToAsync(2, 1000, Easing.SinInOut);
                Task t, o1, o2, o3;

                switch (partName)
                {
                    case "Radosne":
                        t = s.TranslateToAsync(-100, -100, 1000, Easing.SinInOut);
                        o1 = Swiatla.FadeToAsync(1, 1000, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 1000, Easing.SinInOut);
                        break;
                    case "Światła":
                        t = s.TranslateToAsync(-100, +100, 1000, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 1000, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 1000, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        t = s.TranslateToAsync(+100, -100, 1000, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 1000, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 1000, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        t = s.TranslateToAsync(+100, +100, 1000, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 1000, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 1000, Easing.SinInOut);
                        o3 = Bolesne.FadeToAsync(1, 1000, Easing.SinInOut);
                        break;
                }
                
                await Task.WhenAll(scale, t, o1, o2, o3);
                _selectedPart = null;
            }
            else if (_selectedPart!=null)
            {
                return;
            }
            else
            {
                var scale = s.ScaleToAsync(3, 1000, Easing.SinInOut);
                var t = s.TranslateToAsync(0, 0, 1000, Easing.SinInOut);
                Task o1, o2, o3;

                switch (partName)
                {
                    case "Radosne":
                        o1 = Swiatla.FadeToAsync(0, 1000, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 1000, Easing.SinInOut);
                        break;
                    case "Światła":
                        o1 = Radosne.FadeToAsync(0, 1000, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 1000, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        o1 = Radosne.FadeToAsync(0, 1000, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(0, 1000, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 1000, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        o1 = Radosne.FadeToAsync(0, 1000, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(0, 1000, Easing.SinInOut);
                        o3 = Bolesne.FadeToAsync(0, 1000, Easing.SinInOut);
                        break;
                }

                await Task.WhenAll(scale, t, o1, o2, o3);
                _selectedPart = partName;

                await ShowMysteryAnimation(s);
            }
        }

        private async void SetImageAndLabel()
        {
            Mystery1Image.Source = _imagesMap[_selectedPart][0];
            Mystery2Image.Source = _imagesMap[_selectedPart][1];
            Mystery3Image.Source = _imagesMap[_selectedPart][2];
            Mystery4Image.Source = _imagesMap[_selectedPart][3];
            Mystery5Image.Source = _imagesMap[_selectedPart][4];

            //Mystery1Label.Text = _itemsMap[_selectedPart][0];
            //Mystery2Label.Text = _itemsMap[_selectedPart][1];
            //Mystery3Label.Text = _itemsMap[_selectedPart][2];
            //Mystery4Label.Text = _itemsMap[_selectedPart][3];
            //Mystery5Label.Text = _itemsMap[_selectedPart][4];
        }

        private async Task ShowMysteryAnimation(Border s)
        {
            SetImageAndLabel();

            var animationTasks = new List<Task>();
            double radius = s.Height / 2 + 90;
            double center = s.Height / 2;
            double angleOffset = - 2 * Math.PI / 5 - Math.PI / 10;
            int i = 0;
            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                layout.IsVisible = true;
                layout.Opacity = 0;
                var btn = layout.Children.OfType<Border>().FirstOrDefault();
                double btnSize = btn.Width;
                double angle = i * 2 * Math.PI / 5 + angleOffset;
                double tx = center + radius * Math.Cos(angle) - btnSize / 2;
                double ty = center + radius * Math.Sin(angle) - btnSize / 2;
                var btnTranslate = layout.TranslateToAsync(tx, ty, 1000, Easing.SinInOut);
                var btnFade = layout.FadeToAsync(1, 1000, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
                i++;
            }
            await Task.WhenAll(animationTasks);
        }

        private async Task CloseMystryAnimation()
        {
            var animationTasks = new List<Task>();
            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                var btn = layout.Children.OfType<Border>().FirstOrDefault();
                double btnSize = btn.Width;
                var btnTranslate = layout.TranslateToAsync(0, 0, 1000, Easing.SinInOut);
                var btnFade = layout.FadeToAsync(0, 1000, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
            }

            await Task.WhenAll(animationTasks);

            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                layout.IsVisible = false;
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
