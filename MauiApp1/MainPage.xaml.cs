namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public int date;
        public MeditationsService _meditationService;
        public MainPage(MeditationsService meditationService)
        {
            InitializeComponent();
            _meditationService = meditationService;
            UpdateMeditation();
        }

        private async void MyRosaryGroup_Tapped(object sender, TappedEventArgs e)
        {
           

            await Shell.Current.GoToAsync("MyRosaryGroup");
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
            DateLabel.Text ="Dzień "+ date;
            MysteryLabel.Text = savedMystery;
       
            string description = await _meditationService.GetOnlyDescription(this.date, savedMystery);
            MeditationLabel.Text = description ?? "Brak rozważania";
        }
    }
}
