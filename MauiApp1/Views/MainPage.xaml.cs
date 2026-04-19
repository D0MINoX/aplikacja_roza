using MauiApp1.Models;
using MauiApp1.Services;
using System.Data;
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
