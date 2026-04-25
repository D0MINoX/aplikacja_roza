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
            "r1.png",
            "r2.png",
            "r3.png",
            "r4.png",
            "r5.png"
        },
            ["Światła"] = new()
        {
            "s1.png",
            "s2.png",
            "s3.png",
            "s4.png",
            "s5.png"
        },
            ["Bolesne"] = new()
        {
            "b1.png",
            "b2.png",
            "b3.png",
            "b4.png",
            "b5.png"
        },
            ["Chwalebne"] = new()
        {
            "c1.png",
            "c2.png",
            "c3.png",
            "c4.png",
            "c5.png"
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
            await StarterAnimation();
            
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
                btn.TranslationX = btn.TranslationY = 0;
                btn.Scale = 0.33;
                btn.IsVisible = false;
            }
        }

        private async Task StarterAnimation()
        {
            foreach (var btn in new[] { Radosne, Swiatla, Bolesne, Chwalebne })
            {
                btn.TranslationX = btn.TranslationY = 0;
                btn.Opacity = 1;
                btn.Scale = 0.33;
            }

            foreach (var btn in new[] { RadosneLabel, SwiatlaLabel, BolesneLabel, ChwalebneLabel })
            {
                btn.Scale = 0.33;
                btn.Opacity = 1;
            }

            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(750), () =>
            {
                var img = CenterImage.ScaleToAsync(2, 500, Easing.SinInOut);

                var b1 = Radosne.TranslateToAsync(-100, -100, 750, Easing.SinInOut);
                var b1Scale = Radosne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b1Label = RadosneLabel.TranslateToAsync(0, -5, 750, Easing.SinInOut);
                var b1LabelScale = RadosneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b2 = Swiatla.TranslateToAsync(-100, +100, 750, Easing.SinInOut);
                var b2Scale = Swiatla.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b2Label = SwiatlaLabel.TranslateToAsync(0, -5, 750, Easing.SinInOut);
                var b2LabelScale = SwiatlaLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b3 = Bolesne.TranslateToAsync(+100, -100, 750, Easing.SinInOut);
                var b3Scale = Bolesne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b3Label = BolesneLabel.TranslateToAsync(0, -5, 750, Easing.SinInOut);
                var b3LabelScale = BolesneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b4 = Chwalebne.TranslateToAsync(+100, +100, 750, Easing.SinInOut);
                var b4Scale = Chwalebne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b4Label = ChwalebneLabel.TranslateToAsync(0, -5, 750, Easing.SinInOut);
                var b4LabelScale = ChwalebneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                Task.WhenAll(b1, b1Scale, b2, b2Scale, b3, b3Scale, b4, b4Scale, b1Label, b2Label, b3Label, b4Label);
            });

        }

        private async void RosaryPart_Tapped(object sender, TappedEventArgs e)
        {
            Grid s = sender as Grid;
            string partName = e.Parameter.ToString();
            if (_selectedPart == partName)
            {
                await CloseMystryAnimation();
                var scale = s.ScaleToAsync(0.66, 750, Easing.SinInOut);
                Task t, o1, o2, o3, labelScale, labelFade;

                switch (partName)
                {
                    case "Radosne":
                        t = s.TranslateToAsync(-100, -100, 750, Easing.SinInOut);
                        o1 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = RadosneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = RadosneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Światła":
                        t = s.TranslateToAsync(-100, +100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = SwiatlaLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = SwiatlaLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        t = s.TranslateToAsync(+100, -100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = BolesneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = BolesneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        t = s.TranslateToAsync(+100, +100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = ChwalebneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = ChwalebneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                }
                
                await Task.WhenAll(scale, t, o1, o2, o3, labelScale, labelFade);
                _selectedPart = null;
            }
            else if (_selectedPart!=null)
            {
                return;
            }
            else
            {
                Task scale = s.ScaleToAsync(1, 750, Easing.SinInOut);
                Task t = s.TranslateToAsync(0, 30, 750, Easing.SinInOut);
                Task o1, o2, o3, labelScale, labelFade;
                Border border = null;

                switch (partName)
                {
                    case "Radosne":
                        border = RadosneBorder;
                        o1 = Swiatla.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = RadosneLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = RadosneLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Światła":
                        border = SwiatlaBorder;
                        o1 = Radosne.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = SwiatlaLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = SwiatlaLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        border = BolesneBorder;
                        o1 = Radosne.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = BolesneLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = BolesneLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        border = ChwalebneBorder;
                        o1 = Radosne.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Bolesne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = ChwalebneLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = ChwalebneLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                }

                await Task.WhenAll(scale, t, o1, o2, o3, labelScale, labelFade);
                _selectedPart = partName;

                await ShowMysteryAnimation(border);
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
            var btn = Mystery1.Children.OfType<Border>().FirstOrDefault();
            double btnSize = btn.Width;
            double radius = s.Width / 2 + 60;
            double center = s.Width / 2;
            double angleOffset = - 2 * Math.PI / 5 - Math.PI / 10;
            int i = 0;
            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                var lbl = layout.Children.OfType<Label>().FirstOrDefault();
                lbl.Scale = 1;

                layout.IsVisible = true;
                layout.Opacity = 0;

                double angle = i * 2 * Math.PI / 5 + angleOffset;
                double tx = center + radius * Math.Cos(angle) - btnSize / 2;
                double ty = center + radius * Math.Sin(angle) - btnSize / 2 + 20;

                Task btnTranslate = layout.TranslateToAsync(tx, ty, 750, Easing.SinInOut);
                Task btnFade = layout.FadeToAsync(1, 750, Easing.SinInOut);
                Task btnScale = layout.ScaleToAsync(0.66, 750, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
                animationTasks.Add(btnScale);

                Task lblScale = lbl.ScaleToAsync(1.5, 750, Easing.SinInOut);
                Task lblTranslate = lbl.TranslateToAsync(0, -5, 750, Easing.SinInOut);
                animationTasks.Add(lblScale);
                animationTasks.Add(lblTranslate);
                i++;
            }
            await Task.WhenAll(animationTasks);
        }

        private async Task CloseMystryAnimation()
        {
            var animationTasks = new List<Task>();
            var btn = Mystery1.Children.OfType<Border>().FirstOrDefault();
            double btnSize = btn.Width;

            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                Task btnTranslate = layout.TranslateToAsync(0, 0, 750, Easing.SinInOut);
                Task btnFade = layout.FadeToAsync(0, 750, Easing.SinInOut);
                Task btnScale = layout.ScaleToAsync(0.33, 750, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
                animationTasks.Add(btnScale);

                Task lblScale = layout.Children.OfType<Label>().FirstOrDefault().ScaleToAsync(0.33, 750, Easing.SinInOut);
                animationTasks.Add(lblScale);
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
