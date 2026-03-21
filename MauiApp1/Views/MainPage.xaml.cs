using MauiApp1.Models;
using MauiApp1.Services;
using System.Data;
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
        public MainPage(MeditationsService meditationService, AuthService authService, RosaryService rosaryService)
        {
            InitializeComponent();
            _meditationService = meditationService;
            _authService = authService;
            UpdateMeditation();
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
                        if (rosaryInfos != null && rosaryInfos.Count > 0)
                        {
                            rosaryId = rosaryInfos[0].Id;
                            var navigationParameter = new Dictionary<string, object>
                                {
                                    { "RosaryId", rosaryId.ToString() }
                                };
                        await Shell.Current.GoToAsync("MyRosaryGroup", navigationParameter);
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
            MeditationLabel.Text = "Ładowanie ....";
            date = Preferences.Default.Get("LastDate", 1);
            string savedMystery = Preferences.Default.Get("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
            DateLabel.Text = "Dzień " + date;
            MysteryLabel.Text = savedMystery;
            var data = await _meditationService.GetMeditationData(this.date, savedMystery);
          
            MeditationLabel.Text = data.Content ?? "Brak rozważania";
        }
    }
}
