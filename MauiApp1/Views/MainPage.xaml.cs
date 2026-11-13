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

        private const double CompletedOpacity = 0.2;

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
                new MysteryItem("Ukoronowanie", "Cierniem ukoronowanie Pana Jezusa"),
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

        private static readonly Dictionary<string, List<string>> _imagesMap = new()
        {
            ["Radosne"] = new() { "r1.png", "r2.png", "r3.png", "r4.png", "r5.png" },
            ["Światła"] = new() { "s1.png", "s2.png", "s3.png", "s4.png", "s5.png" },
            ["Bolesne"] = new() { "b1.png", "b2.png", "b3.png", "b4.png", "b5.png" },
            ["Chwalebne"] = new() { "c1.png", "c2.png", "c3.png", "c4.png", "c5.png" }
        };

        private string _selectedPart = null;
        private bool _firstload;
        private bool _resetMenuOnReturn;
        private bool _transitionInProgress;
        private string? _calendarPart;
        private Window? _observedWindow;

        public MainPage(MeditationsService meditationService, AuthService authService, RosaryService rosaryService)
        {
            InitializeComponent();
            _meditationService = meditationService;
            _authService = authService;
            _rosaryService = rosaryService;
            _firstload = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (Window is { } window && _observedWindow == null)
            {
                _observedWindow = window;
                window.Resumed += OnWindowResumed;
            }
            if (Preferences.Get("app_main_theme", false))
            {
                background.Source = "mainpagebackgrounddark.png";
            }
            else
            {
                background.Source = "mainpagebackground.png";
            }

            bool firstLoad = _firstload;
            _firstload = false;
            if (ResetExpiredCalendar())
                return;

            if (!CalendarContainer.IsVisible && TryRestoreCalendar())
            {
                _resetMenuOnReturn = false;
                return;
            }

            if (firstLoad)
            {
                _transitionInProgress = true;
                try { await StarterAnimation(); }
                finally { _transitionInProgress = false; }
            }

            if (_resetMenuOnReturn)
            {
                _resetMenuOnReturn = false;
                ResetRosaryMenu();
            }
        }

        protected override void OnDisappearing()
        {
            if (_observedWindow is { } window)
            {
                window.Resumed -= OnWindowResumed;
                _observedWindow = null;
            }
            base.OnDisappearing();
        }

        private void OnWindowResumed(object? sender, EventArgs e)
        {
            if (!_transitionInProgress)
                ResetExpiredCalendar();
        }

        private bool ResetExpiredCalendar()
        {
            if (!RosaryCycle.ExpireIfDue(DateTime.Today))
                return false;

            _resetMenuOnReturn = false;
            ResetRosaryMenu();
            return true;
        }

        private bool TryRestoreCalendar()
        {
            if (!RosaryCycle.CanRestore(DateTime.Today))
                return false;

            string savedMystery = Preferences.Default.Get("LastMystery", string.Empty);
            foreach (var (part, mysteries) in _itemsMap)
            {
                int index = mysteries.FindIndex(mystery =>
                    string.Equals(mystery.FullDescription, savedMystery, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                    continue;

                ResetRosaryMenu();
                foreach (var partGrid in new[] { Radosne, Swiatla, Bolesne, Chwalebne })
                {
                    partGrid.Opacity = 0;
                    partGrid.InputTransparent = true;
                }

                CenterImage.IsVisible = false;
                ShowCalendar(part, index, opacity: 1, scale: 1);
                return true;
            }

            return false;
        }

        private void ResetRosaryMenu()
        {
            _selectedPart = null;
            _calendarPart = null;
            CalendarContainer.IsVisible = false;
            CalendarContainer.InputTransparent = false;
            SelectedMysteryPreview.IsVisible = false;
            SelectedMysteryPreview.Opacity = 0;

            foreach (var mystery in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                mystery.TranslationX = mystery.TranslationY = 0;
                mystery.Scale = 0.33;
                mystery.Opacity = 0;
            }

            foreach (var (part, label, x, y) in new[]
            {
                (Radosne, RadosneLabel, 100, -100),
                (Swiatla, SwiatlaLabel, 100, 100),
                (Bolesne, BolesneLabel, -100, 100),
                (Chwalebne, ChwalebneLabel, -100, -100)
            })
            {
                part.TranslationX = x;
                part.TranslationY = y;
                part.Scale = 0.66;
                part.Opacity = 1;
                part.InputTransparent = false;
                label.Scale = 1.5;
                label.Opacity = 1;
            }

            CenterImage.Scale = 2;
            CenterImage.Opacity = 1;
            CenterImage.IsVisible = true;
        }

        private async Task StarterAnimation()
        {
            foreach (var btn in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                btn.TranslationX = btn.TranslationY = 0;
                btn.Scale = 0.33;
                btn.Opacity = 0;
            }
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

            await Task.Delay(750);
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

            await Task.WhenAll(logo, img, b1, b1Scale, b2, b2Scale, b3, b3Scale, b4, b4Scale,
                b1Fade, b2Fade, b3Fade, b4Fade, b1LabelScale, b2LabelScale, b3LabelScale, b4LabelScale);
        }

        private async void RosaryPart_Tapped(object sender, TappedEventArgs e)
        {
            if (sender is not Grid partGrid || e.Parameter is not string partName ||
                CalendarContainer.IsVisible || _transitionInProgress)
                return;

            ResetExpiredCalendar();
            await RunTransitionAsync(() => ToggleRosaryPartAsync(partGrid, partName));
        }

        private async Task RunTransitionAsync(Func<Task> transition)
        {
            _transitionInProgress = true;
            try { await transition(); }
            finally { _transitionInProgress = false; }
        }

        private async Task ToggleRosaryPartAsync(Grid s, string partName)
        {
            if (_selectedPart == partName)
            {
                await CloseMysteryAnimation();

                Radosne.InputTransparent = false;
                Swiatla.InputTransparent = false;
                Bolesne.InputTransparent = false;
                Chwalebne.InputTransparent = false;

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

                if (!CenterImage.IsVisible)
                {
                    CenterImage.IsVisible = true;
                    await CenterImage.FadeToAsync(1, 400, Easing.SinInOut);
                }
            }
            else if (_selectedPart != null)
            {
                return;
            }
            else
            {
                Task scale = s.ScaleToAsync(1, 750, Easing.SinInOut);
                Task t = s.TranslateToAsync(0, 30, 750, Easing.SinInOut);
                Task o1, o2, o3, labelScale, labelFade;
                Border border = null;

                Radosne.InputTransparent = true;
                Swiatla.InputTransparent = true;
                Bolesne.InputTransparent = true;
                Chwalebne.InputTransparent = true;

                switch (partName)
                {
                    case "Radosne":
                        Radosne.InputTransparent = false;
                        border = RadosneBorder;
                        o1 = Swiatla.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = RadosneLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = RadosneLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Światła":
                        Swiatla.InputTransparent = false;
                        border = SwiatlaBorder;
                        o1 = Radosne.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Bolesne.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = SwiatlaLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = SwiatlaLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Bolesne":
                        Bolesne.InputTransparent = false;
                        border = BolesneBorder;
                        o1 = Radosne.FadeToAsync(0, 750, Easing.SinInOut);
                        o2 = Swiatla.FadeToAsync(0, 750, Easing.SinInOut);
                        o3 = Chwalebne.FadeToAsync(0, 750, Easing.SinInOut);
                        labelScale = BolesneLabel.ScaleToAsync(1, 750, Easing.SinInOut);
                        labelFade = BolesneLabel.FadeToAsync(0, 750, Easing.SinInOut);
                        break;
                    case "Chwalebne":
                    default:
                        Chwalebne.InputTransparent = false;
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

        private void SetImageAndLabel()
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

        private int GetSavedMysteryIndex(string part)
        {
            var savedMystery = Preferences.Default.Get("LastMystery", string.Empty);
            return _itemsMap[part].FindIndex(item =>
                string.Equals(item.FullDescription, savedMystery, StringComparison.OrdinalIgnoreCase));
        }

        private async Task ShowMysteryAnimation(Border s)
        {
            SetImageAndLabel();

            var animationTasks = new List<Task>();
            var btn = Mystery1.Children.OfType<Border>().FirstOrDefault();
            double btnSize = btn.Width;
            double radius = s.Width / 2 + 80;
            double center = s.Width / 2;
            double angleOffset = -2 * Math.PI / 5 - Math.PI / 10;
            int i = 0;
            int savedIndex = GetSavedMysteryIndex(_selectedPart);

            foreach (var layout in new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 })
            {
                var lbl = layout.Children.OfType<Label>().FirstOrDefault();
                lbl.Scale = 1;
                layout.Opacity = 0;

                double angle = i * 2 * Math.PI / 5 + angleOffset;
                double tx = center + radius * Math.Cos(angle) - btnSize / 2;
                double ty = center + radius * Math.Sin(angle) - btnSize / 2 + 20;

                Task btnTranslate = layout.TranslateToAsync(tx, ty, 750, Easing.SinInOut);
                Task btnFade = layout.FadeToAsync(i < savedIndex ? CompletedOpacity : 1, 750, Easing.SinInOut);
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

        private async Task CloseMysteryAnimation()
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
            if (_selectedPart == null || _transitionInProgress || CalendarContainer.IsVisible)
                return;
            if (ResetExpiredCalendar())
                return;

            int mysteryNumber = int.Parse(e.Parameter.ToString());
            await RunTransitionAsync(() => OpenCalendarAsync(mysteryNumber - 1));
        }

        private async Task OpenCalendarAsync(int index)
        {
            string part = _selectedPart;
            string newMystery = _itemsMap[part][index].FullDescription;

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

            if (CenterImage.IsVisible)
            {
                hideTasks.Add(CenterImage.FadeToAsync(0, 300, Easing.SinInOut));
            }

            await Task.WhenAll(hideTasks);
            CenterImage.IsVisible = false;

            RosaryCycle.SelectMystery(newMystery, DateTime.Today);
            ShowCalendar(part, index, opacity: 0, scale: 0.8);

            await Task.WhenAll(
                CalendarContainer.FadeToAsync(1, 450, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(1, 450, Easing.SinInOut)
            );
        }

        private void ShowCalendar(string part, int index, double opacity, double scale)
        {
            _calendarPart = part;
            SelectedMysteryImage.Source = _imagesMap[part][index];
            SelectedMysteryLabel.Text = _itemsMap[part][index].ShortLabel;
            GenerateCalendarGrid();

            SelectedMysteryPreview.IsVisible = true;
            SelectedMysteryPreview.Opacity = 1;
            SelectedMysteryPreview.Scale = 0.8;

            CalendarContainer.Opacity = opacity;
            CalendarContainer.Scale = scale;
            CalendarContainer.InputTransparent = false;
            CalendarContainer.IsVisible = true;
        }

        private async void OnBackToMysteries_Tapped(object sender, TappedEventArgs e)
        {
            if (!_transitionInProgress && CalendarContainer.IsVisible && !ResetExpiredCalendar())
                await RunTransitionAsync(CloseCalendarAsync);
        }

        protected override bool OnBackButtonPressed() => TryGoBack() || base.OnBackButtonPressed();

        private void Back_Swiped(object sender, SwipedEventArgs e) => TryGoBack();

        private bool TryGoBack()
        {
            if (_transitionInProgress || ResetExpiredCalendar())
                return true;

            if (CalendarContainer.IsVisible)
                _ = RunTransitionAsync(CloseCalendarAsync);
            else if (_selectedPart is { } part)
                _ = RunTransitionAsync(() => ToggleRosaryPartAsync(GetPartGrid(part), part));
            else
                return false;

            return true;
        }

        private Grid GetPartGrid(string part) => part switch
        {
            "Radosne" => Radosne,
            "Światła" => Swiatla,
            "Bolesne" => Bolesne,
            "Chwalebne" => Chwalebne,
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };

        private async Task CloseCalendarAsync()
        {
            await Task.WhenAll(
                CalendarContainer.FadeToAsync(0, 250, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(0.66, 250, Easing.SinInOut)
            );

            CalendarContainer.IsVisible = false;
            SelectedMysteryPreview.IsVisible = false;

            if (_selectedPart == null)
            {
                string? part = _calendarPart;
                ResetRosaryMenu();
                if (part != null)
                    await ToggleRosaryPartAsync(GetPartGrid(part), part);
                return;
            }

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

            showMenuTasks.Add(CenterImage.FadeToAsync(1, 250, Easing.SinInOut));

            int savedIndex = GetSavedMysteryIndex(_selectedPart);
            var layouts = new[] { Mystery1, Mystery2, Mystery3, Mystery4, Mystery5 };
            for (int i = 0; i < layouts.Length; i++)
            {
                var layout = layouts[i];
                layout.Scale = .66;
                showMenuTasks.Add(layout.FadeToAsync(i < savedIndex ? CompletedOpacity : 1, 300, Easing.SinInOut));
            }

            await Task.WhenAll(showMenuTasks);
        }

        private async void MyRosaryGroup_Tapped(object sender, TappedEventArgs e)
        {
            if (string.IsNullOrEmpty(_authService.Token)) return;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(_authService.Token);
            var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
            int userRole = int.Parse(roleClaim?.Value ?? "5");

            if (userRole == 0 || userRole == 1)
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
                        var navigationParameter = new Dictionary<string, object> { { "RosaryId", rosaryId.ToString() }, { "UserRole", userRole } };
                        await Shell.Current.GoToAsync("MyRosaryGroup", navigationParameter);
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("SelectParish", new Dictionary<string, object> { { "UserId", Id } });
                    }
                }
            }
        }

        private void GenerateCalendarGrid()
        {
            CalendarGrid.Children.Clear();

            int columnsCount = 6;
            int lastDate = Preferences.Get("LastCompleteDate", -1);

            // =========================
            // WPROWADZENIE - cały wiersz
            // =========================

            var borderWprowadzenie = new Border
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };

            if (App.Current.Resources.TryGetValue("AppCalendarDayButton", out var borderStyleWprowadzenie))
            {
                borderWprowadzenie.Style = (Style)borderStyleWprowadzenie;
            }

            var labelWprowadzenie = new Label
            {
                Text = "Wprowadzenie",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            if (App.Current.Resources.TryGetValue("AppCalendarDayLabel", out var labelStyleWprowadzenie))
            {
                labelWprowadzenie.Style = (Style)labelStyleWprowadzenie;
            }

            borderWprowadzenie.Content = labelWprowadzenie;

            if (lastDate >= 0)
            {
                borderWprowadzenie.Opacity = CompletedOpacity;
            }

            var tapGestureWprowadzenie = new TapGestureRecognizer();

            tapGestureWprowadzenie.Tapped += async (s, e) =>
            {
                await borderWprowadzenie.ScaleToAsync(0.9, 50, Easing.Linear);
                await borderWprowadzenie.ScaleToAsync(1.0, 50, Easing.Linear);

                HandleDaySelection(0);
            };

            borderWprowadzenie.GestureRecognizers.Add(tapGestureWprowadzenie);

            Grid.SetRow(borderWprowadzenie, 0);
            Grid.SetColumn(borderWprowadzenie, 0);
            Grid.SetColumnSpan(borderWprowadzenie, 6);

            CalendarGrid.Children.Add(borderWprowadzenie);


            // =========================
            // DNI 1 - 30
            // =========================

            for (int dayNumber = 1; dayNumber <= 30; dayNumber++)
            {
                int index = dayNumber - 1;

                int row = (index / columnsCount) + 1;
                int col = index % columnsCount;

                var border = new Border
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };

                if (App.Current.Resources.TryGetValue("AppCalendarDayButton", out var borderStyle))
                {
                    border.Style = (Style)borderStyle;
                }

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

                if (dayNumber <= lastDate)
                {
                    border.Opacity = CompletedOpacity;
                }

                int selectedDay = dayNumber;

                var tapGesture = new TapGestureRecognizer();

                tapGesture.Tapped += async (s, e) =>
                {
                    await border.ScaleToAsync(0.9, 50, Easing.Linear);
                    await border.ScaleToAsync(1.0, 50, Easing.Linear);

                    HandleDaySelection(selectedDay);
                };

                border.GestureRecognizers.Add(tapGesture);

                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

                CalendarGrid.Children.Add(border);
            }


            // =========================
            // DODATEK - dzień 31
            // cały dolny wiersz
            // =========================

            int dodatekDay = 31;

            // 30 dni / 6 kolumn = 5 wierszy
            // Wprowadzenie = row 0
            // Dni = row 1-5
            // Dodatek = row 6
            int dodatekRow = 6;

            var borderDodatek = new Border
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                
            };

            if (App.Current.Resources.TryGetValue("AppCalendarDayButton", out var borderStyleDodatek))
            {
                borderDodatek.Style = (Style)borderStyleDodatek;
            }

            var labelDodatek = new Label
            {
                Text = "Dodatek",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
                
            };

            if (App.Current.Resources.TryGetValue("AppCalendarDayLabel", out var labelStyleDodatek))
            {
                labelDodatek.Style = (Style)labelStyleDodatek;
            }

            borderDodatek.Content = labelDodatek;

            if (dodatekDay <= lastDate)
            {
                borderDodatek.Opacity = CompletedOpacity;
            }

            var tapGestureDodatek = new TapGestureRecognizer();

            tapGestureDodatek.Tapped += async (s, e) =>
            {
                await borderDodatek.ScaleToAsync(0.9, 50, Easing.Linear);
                await borderDodatek.ScaleToAsync(1.0, 50, Easing.Linear);

                HandleDaySelection(31);
            };

            borderDodatek.GestureRecognizers.Add(tapGestureDodatek);

            Grid.SetRow(borderDodatek, 6);
            Grid.SetColumn(borderDodatek, 0);
            Grid.SetColumnSpan(borderDodatek, 6);

            CalendarGrid.Children.Add(borderDodatek);
        }

        private async void HandleDaySelection(int wybranyDzien)
        {
            if (!CalendarContainer.IsVisible || CalendarContainer.InputTransparent ||
                _transitionInProgress || ResetExpiredCalendar())
                return;

            await RunTransitionAsync(() => OpenDayAsync(wybranyDzien));
        }

        private async Task OpenDayAsync(int day)
        {
            CalendarContainer.InputTransparent = true;
            this.date = day;
            RosaryCycle.SelectDay(date, DateTime.Today);

            await Task.WhenAll(
                CalendarContainer.FadeToAsync(0, 300, Easing.SinInOut),
                CalendarContainer.ScaleToAsync(0.8, 300, Easing.SinInOut)
            );

            CalendarContainer.IsVisible = false;
            SelectedMysteryPreview.IsVisible = false;
            _resetMenuOnReturn = true;
            await Shell.Current.GoToAsync("FullMeditation");
        }
    }
}
