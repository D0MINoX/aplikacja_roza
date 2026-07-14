 using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public int date;
        public MeditationsService _meditationService;
        public AuthService _authService;
        public RosaryService _rosaryService;
        private static readonly Dictionary<string, List<MysteryItem>> _itemsMap = new()
        {
            ["Radosne"] = new()
    {
        new MysteryItem("Zwiastowanie", "Zwiastowanie Najświętszej Maryi Pannie"),
        new MysteryItem("Nawiedzenie", "Nawiedzenie św. Elżbiety"),
        new MysteryItem("Narodzenie", "Narodzenie Pana Jezusa"),
        new MysteryItem("Ofiarowanie", "Ofiarowanie Pana Jezusa w świątyni"),
        new MysteryItem("Odnalezienie", "Odnalezienie Pana Jezusa w świątyni")
    },
            ["Światła"] = new()
    {
        new MysteryItem("Chrzest", "Chrzest Pana Jezusa w Jordanie"),
        new MysteryItem("Kana", "Objawienie się Pana Jezusa w Kanie Galilejskiej"),
        new MysteryItem("Królestwo", "Głoszenie Królestwa Bożego i wzywanie do nawrócenia"),
        new MysteryItem("Przemienienie", "Przemienienie na górze Tabor"),
        new MysteryItem("Eucharystia", "Ustanowienie Eucharystii")
    },
            ["Bolesne"] = new()
    {
        new MysteryItem("Modlitwa", "Modlitwa Pana Jezusa w Ogrójcu"),
        new MysteryItem("Biczowanie", "Biczowanie Pana Jezusa"),
        new MysteryItem("ukoronowanie", "Cierniem ukoronowanie Pana Jezusa"),
        new MysteryItem("Dźwiganie krzyża", "Dźwiganie krzyża na Kalwarię"),
        new MysteryItem("Ukrzyżowanie", "Ukrzyżowanie i śmierć Pana Jezusa")
    },
            ["Chwalebne"] = new()
    {
        new MysteryItem("Zmartwychwstanie", "Zmartwychwstanie Pana Jezusa"),
        new MysteryItem("Wniebowstąpienie", "Wniebowstąpienie Pana Jezusa"),
        new MysteryItem("Zesłanie", "Zesłanie Ducha Świętego"),
        new MysteryItem("Wniebowzięcie", "Wniebowzięcie Najświętszej Maryi Panny"),
        new MysteryItem("Ukoronowanie", "Ukoronowanie Najświętszej Maryi Panny na Królową Nieba i Ziemi")
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
            GenerateCalendarGrid();
           
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _selectedPart = null;
            await StarterAnimation();
            
            //bool hasToken = await _authService.CheckAndSetTokenAsync();
            //if (hasToken)
            //{
            //    RosaryTile.IsVisible = true;
            //}
            //else
            //{
            //    RosaryTile.IsVisible = false;
            //}
            //await UpdateMeditation();

            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                btn.TranslationX = btn.TranslationY = 0;
                btn.Scale = 0.33;
                btn.Opacity = 0;
            }
        }

        private async Task StarterAnimation()
        {
            foreach (var btn in new[] { Radosne, Swiatla, Bolesne, Chwalebne })
            {
                btn.TranslationX = btn.TranslationY = 0;
                btn.Opacity = 0;
                btn.Scale = 0.33;
            }

            foreach (var btn in new[] { RadosneLabel, SwiatlaLabel, BolesneLabel, ChwalebneLabel })
            {
                btn.Scale = 0.33;
                btn.Opacity = 1;
            }
            if (!CenterImage.IsVisible)
            {
                CenterImage.IsVisible = true;
                await CenterImage.FadeToAsync(1, 400, Easing.SinInOut);
            }
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(750), () =>
            {
                var b1Fade = Radosne.FadeToAsync(1, 500, Easing.SinInOut);
                var b2Fade = Swiatla.FadeToAsync(1, 500, Easing.SinInOut);
                var b3Fade = Bolesne.FadeToAsync(1, 500, Easing.SinInOut);
                var b4Fade = Chwalebne.FadeToAsync(1, 500, Easing.SinInOut);

                var img = CenterImage.ScaleToAsync(2, 500, Easing.SinInOut);

                var b1 = Radosne.TranslateToAsync(+100, -100, 750, Easing.SinInOut);
                var b1Scale = Radosne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b1LabelScale = RadosneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b2 = Swiatla.TranslateToAsync(+100, +100, 750, Easing.SinInOut);
                var b2Scale = Swiatla.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b2LabelScale = SwiatlaLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b3 = Bolesne.TranslateToAsync(-100, +100, 750, Easing.SinInOut);
                var b3Scale = Bolesne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b3LabelScale = BolesneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);

                var b4 = Chwalebne.TranslateToAsync(-100, -100, 750, Easing.SinInOut);
                var b4Scale = Chwalebne.ScaleToAsync(0.66, 750, Easing.SinInOut);
                var b4LabelScale = ChwalebneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                var logo = CenterImage.FadeToAsync(1, 400, Easing.SinInOut);
                CenterImage.IsVisible = true;
                Task.WhenAll(logo,b1, b1Scale, b2, b2Scale, b3, b3Scale, b4, b4Scale, b1Fade, b2Fade, b3Fade, b4Fade);
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
                        t = s.TranslateToAsync(+100, -100, 750, Easing.SinInOut);
                        o1 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = RadosneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = RadosneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Światła":
                        t = s.TranslateToAsync(+100, +100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = SwiatlaLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = SwiatlaLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        t = s.TranslateToAsync(-100, +100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = BolesneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = BolesneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        t = s.TranslateToAsync(-100, -100, 750, Easing.SinInOut);
                        o1 = Radosne.FadeToAsync(1, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(1, 750, Easing.SinInOut);
                        o3 = Bolesne.FadeToAsync(1, 750, Easing.SinInOut);
                        labelScale = ChwalebneLabel.ScaleToAsync(1.5, 750, Easing.SinInOut);
                        labelFade = ChwalebneLabel.FadeToAsync(1, 750, Easing.SinInOut);
                        break;
                }
                
                await Task.WhenAll(scale, t, o1, o2, o3, labelScale, labelFade);
                _selectedPart = null;
                if(!CenterImage.IsVisible)
                {
                    CenterImage.IsVisible = true;
                    await CenterImage.FadeToAsync(1, 400, Easing.SinInOut);
                }
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
                if (CenterImage.IsVisible)
                {
                    await CenterImage.FadeToAsync(0, 400, Easing.SinInOut);
                    CenterImage.IsVisible = false;
                }
                
                await Task.WhenAll(scale, t, o1, o2, o3, labelScale, labelFade);
                _selectedPart = partName;
                ThemeManager.SetTheme(partName);
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

            Mystery1Label.Text = _itemsMap[_selectedPart][0].ShortLabel;
            Mystery2Label.Text = _itemsMap[_selectedPart][1].ShortLabel;
            Mystery3Label.Text = _itemsMap[_selectedPart][2].ShortLabel;
            Mystery4Label.Text = _itemsMap[_selectedPart][3].ShortLabel;
            Mystery5Label.Text = _itemsMap[_selectedPart][4].ShortLabel;
        }

         private async Task ShowMysteryAnimation(Border s)
         {

             SetImageAndLabel();

             var animationTasks = new List<Task>();
             var btn = Mystery1.Children.OfType<Border>().FirstOrDefault();
             double btnSize = btn.Width;
             double radius = s.Width / 2 + 80;
             double center = s.Width / 2;
             double angleOffset = - 2 * Math.PI / 5 - Math.PI / 10;
             int i = 0;
             foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
             {
                 var lbl = layout.Children.OfType<Label>().FirstOrDefault();
                 lbl.Scale = 1;

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

            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                Task btnTranslate = layout.TranslateToAsync(0, 0, 750, Easing.SinInOut);
                Task btnFade = layout.FadeToAsync(0, 750, Easing.SinInOut);
                Task btnScale = layout.ScaleToAsync(0.33, 750, Easing.SinInOut);
                animationTasks.Add(btnTranslate);
                animationTasks.Add(btnFade);
                animationTasks.Add(btnScale);

                Label lbl = layout.Children.OfType<Label>().FirstOrDefault();
                Task lblScale = lbl.ScaleToAsync(1.5, 750, Easing.SinInOut);
                animationTasks.Add(lblScale);
            }

            await Task.WhenAll(animationTasks);
        }

        private async void Mystery_Tapped(object sender, TappedEventArgs e)
        {
            // Bezpiecznik na starcie - jeśli brak wybranej części, nie robimy nic
            if (string.IsNullOrEmpty(_selectedPart)) return;

            int mysteryNumber = int.Parse(e.Parameter.ToString());
            int index = mysteryNumber - 1; // Indeks listy (0-4)

            List<MysteryItem> mysteries = _itemsMap[_selectedPart];

            // 1. Przypisujemy dane wybranej tajemnicy do kafelka podglądu nad kalendarzem
            if (_imagesMap.ContainsKey(_selectedPart))
            {
                SelectedMysteryImage.Source = _imagesMap[_selectedPart][index];
            }
            SelectedMysteryLabel.Text = mysteries[index].ShortLabel;

            // 2. Przygotowujemy zadania ukrywania obecnych elementów
            var hideTasks = new List<Task>();

            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                hideTasks.Add(layout.FadeToAsync(0, 300, Easing.SinInOut));
                hideTasks.Add(layout.ScaleToAsync(0.33, 300, Easing.SinInOut));
            }

            Grid activeRosaryGrid = _selectedPart switch
            {
                "Radosne" => Radosne,
                "Światła" => Swiatla,
                "Bolesne" => Bolesne,
                "Chwalebne" => Chwalebne,
                _ => null
            };

            if (activeRosaryGrid != null)
            {
                hideTasks.Add(activeRosaryGrid.FadeToAsync(0, 300, Easing.SinInOut));
            }

            // Ukrywamy logo w tle
            if (CenterImage.IsVisible)
            {
                hideTasks.Add(CenterImage.FadeToAsync(0, 300, Easing.SinInOut));
            }

            // Czekamy na zakończenie animacji znikania
            await Task.WhenAll(hideTasks);
            CenterImage.IsVisible = false;

            // 3. Zapisujemy wybraną tajemnicę w pamięci urządzenia
            Preferences.Default.Set("LastMystery", mysteries[index].FullDescription);

            // 4. Przygotowujemy elementy wewnątrz kontenera (upewniamy się, że mają Opacity = 1)
            SelectedMysteryPreview.IsVisible = true;
            SelectedMysteryPreview.Opacity = 1;
            SelectedMysteryPreview.Scale = 0.8;

          

            // 5. Pokazujemy CAŁY kontener (Kafelek tajemnicy + Kalendarz)
            CalendarContainer.IsVisible = true;
            CalendarContainer.Scale = 0.8;

            await Task.WhenAll(
                CalendarContainer.FadeToAsync(1, 450, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(1, 450, Easing.SinInOut)
            );
        }

        /*private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is int wybranyDzien)
            {
                this.date = wybranyDzien;
                Preferences.Default.Set("LastDate", date);

                CalendarSelectionView.SelectedItem = null;

                // FIX: Animujemy i ukrywamy CAŁY KONTENER, a nie pojedyncze elementy wewnątrz.
                // Dzięki temu nie psujemy wartości Opacity dla samej siatki kalendarza.
                await Task.WhenAll(
                    CalendarContainer.FadeToAsync(0, 300, Easing.SinInOut),
                    CalendarContainer.ScaleToAsync(0.8, 300, Easing.SinInOut)
                );

                // Po animacji chowamy logicznie cały kontener oraz elementy wewnętrzne
                CalendarContainer.IsVisible = false;
                CalendarSelectionView.IsVisible = false;
                SelectedMysteryPreview.IsVisible = false;
                SelectedMysteryPreview.Opacity = 0;

                // Resetujemy stan kafelków tajemnic wokół środka
                foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
                {
                    layout.Scale = 1;
                    layout.Opacity = 0;
                }

                // Przygotowujemy logo do ponownego pokazania przez StarterAnimation
                CenterImage.IsVisible = true;
                CenterImage.Opacity = 0;

                _selectedPart = null;

                // Przejście do widoku pełnej medytacji
                await Shell.Current.GoToAsync("FullMeditation");

                // Przywracanie widoku w tle (na wypadek powrotu z FullMeditation wstecz)
                await CloseMystryAnimation();
                await StarterAnimation();
            }
        }
*/
        private async void OnBackToMysteries_Tapped(object sender, TappedEventArgs e)
        {
            // 1. Płynnie ukrywamy kontener z kalendarzem i podglądem tajemnicy
            await Task.WhenAll(
                CalendarContainer.FadeToAsync(0, 250, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(0.66, 250, Easing.SinInOut)
            );

            CalendarContainer.IsVisible = false;
          
            SelectedMysteryPreview.IsVisible = false;

            // 2. Przywracamy widoczność aktywnej części różańca (tekst na dole/górze ekranu)
            Grid activeRosaryGrid = _selectedPart switch
            {
                "Radosne" => Radosne,
                "Światła" => Swiatla,
                "Bolesne" => Bolesne,
                "Chwalebne" => Chwalebne,
                _ => null
            };

            var showMenuTasks = new List<Task>();

            if (activeRosaryGrid != null)
            {
                showMenuTasks.Add(activeRosaryGrid.FadeToAsync(1, 250, Easing.SinInOut));
            }

            // 3. Przywracamy małe logo w środku (bo wokół niego kręcą się tajemnice)
           // CenterImage.IsVisible = true;
            showMenuTasks.Add(CenterImage.FadeToAsync(1, 250, Easing.SinInOut));

            // 4. Resetujemy pozycję i płynnie przywracamy widok 5 tajemnic (Mystery1 - Mystery5)
            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                layout.Scale = .66; // Przywracamy domyślną skalę sprzed ukrycia
                showMenuTasks.Add(layout.FadeToAsync(1, 300, Easing.SinInOut));
            }

            // Odpalamy animacje powrotu do widoku wyboru tajemnic
            await Task.WhenAll(showMenuTasks);
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
        private void GenerateCalendarGrid()
        {
            CalendarGrid.Children.Clear();

            int totalItems = 32; // Liczba dni: od 0 do 31
            int columnsCount = 6; // 6 kolumn w rzędzie

            for (int i = 0; i < totalItems; i++)
            {
                int dayNumber = i; // Lokalna kopia zmiennej dla obsługi kliknięcia
                int row = i / columnsCount;
                int col = i % columnsCount;

                // 1. Tworzymy Border (Klocek dnia)
                var border = new Border
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                // Pobieramy dynamiczny styl przypisany w projekcie
                if (App.Current.Resources.TryGetValue("AppCalendarDayButton", out var borderStyle))
                {
                    border.Style = (Style)borderStyle;
                }

                // 2. Tworzymy Label wewnątrz klocka
                var label = new Label
                {
                    Text = dayNumber.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                if (App.Current.Resources.TryGetValue("AppCalendarDayLabel", out var labelStyle))
                {
                    label.Style = (Style)labelStyle;
                }

                border.Content = label;

                // 3. Dodajemy obsługę kliknięcia w kafelek (Zastępuje dawne SelectionChanged)
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    // Efekt kliknięcia (opcjonalne mikro-skalowanie dla responsywności)
                    await border.ScaleTo(0.9, 50, Easing.Linear);
                    await border.ScaleTo(1.0, 50, Easing.Linear);

                    HandleDaySelection(dayNumber);
                };
                border.GestureRecognizers.Add(tapGesture);

                // 4. Przypisujemy pozycję w siatce i dodajemy do widoku
                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);
                CalendarGrid.Children.Add(border);
            }
        }
        private async void HandleDaySelection(int wybranyDzien)
        {
            this.date = wybranyDzien;
            Preferences.Default.Set("LastDate", date);

            // Ukrywamy cały kontener kalendarza
            await Task.WhenAll(
                CalendarContainer.FadeToAsync(0, 300, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(0.8, 300, Easing.SinInOut)
            );

            CalendarContainer.IsVisible = false;
            SelectedMysteryPreview.IsVisible = false;
            SelectedMysteryPreview.Opacity = 0;

            // Reset kafelków tajemnic wokół środka
            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                layout.Scale = 0.33;
                layout.Opacity = 0;
                layout.TranslationX = 0;
                layout.TranslationY = 0;
            }

            // Przygotowujemy stan widoku głównego pod powrót
            _selectedPart = null;

            // Przejście do widoku pełnej medytacji
            await Shell.Current.GoToAsync("FullMeditation");

            // Po powrocie z medytacji, odpalamy czysty powrót do ekranu wyjściowego
            await CloseMystryAnimation();
            await StarterAnimation();
        }
        private async void RosaryMeditations_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("RosaryMeditations");
        }

        //private async void Meditation_Tapped(object sender, TappedEventArgs e)
        //{
        //    string textToSend = MeditationLabel.Text;

        //    if (string.IsNullOrWhiteSpace(textToSend) || textToSend == "Brak rozważania")
        //        return;

        //    var navigationParameter = new Dictionary<string, object>
        //    {
        //        { "MeditationContent", textToSend }
        //    };
        //    await Shell.Current.GoToAsync("FullMeditation", navigationParameter);
        //}

        //private async Task UpdateMeditation()
        //{
        //    try
        //    {
               
        //        date=Preferences.Default.Get("LastDate", 1);
                
        //        string selectedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
        //        if (string.IsNullOrEmpty(selectedMystery)) return;

        //        MeditationLabel.Text = "Ładowanie ....";


        //        var localData = await GetMeditationFromLocalFile(this.date, selectedMystery);

        //        if (localData != null)
        //        {
        //            DateLabel.Text = "Dzień " + this.date;
        //            MysteryLabel.Text = selectedMystery;
        //            MeditationLabel.Text = localData?.Content ?? "Brak rozważania";
                  
        //            return;
        //        }

        //        var data = await _meditationService.GetMeditationData(this.date, selectedMystery);
        //        DateLabel.Text = "Dzień " + this.date;
        //        MysteryLabel.Text = selectedMystery;
        //        MeditationLabel.Text = data?.Content ?? "Brak rozważania";
        //    }
        //    catch (Exception ex)
        //    {
        //        MeditationLabel.Text = "Błąd połączenia";
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }
        //}

        //private async Task<LocalMeditation> GetMeditationFromLocalFile(int day, string mystery)
        //{
        //    try
        //    {

        //        string path = GetFileName(mystery);
        //        if (!File.Exists(path)) return null;

        //        string json = await File.ReadAllTextAsync(path);
        //        var allMeditations = JsonSerializer.Deserialize<List<LocalMeditation>>(json);

        //        return allMeditations?.FirstOrDefault(m => m.Date == day);
        //    }
        //    catch { return null; }
        //}

        //private string GetFileName(string mystery)
        //{
        //    string safeName = mystery.Replace(" ", "_").Substring(0, Math.Min(mystery.Length, 20));
        //    return Path.Combine(FileSystem.AppDataDirectory, $"meditations_{safeName}.json");
        //}
    }
}
