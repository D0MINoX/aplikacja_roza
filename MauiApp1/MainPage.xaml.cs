namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
       
        public MainPage()
        {
            InitializeComponent();
        }

        private async void MyRosaryGroup_Tapped(object sender, TappedEventArgs e)
        {
            var current = Shell.Current?.CurrentState?.Location?.ToString() ?? "";

            if (current.Contains("MyRosaryGroup", StringComparison.OrdinalIgnoreCase))
                return;

            await Shell.Current.GoToAsync("MyRosaryGroup");
        }
        private async void RosaryMeditations_Tapped(object sender, TappedEventArgs e)
        {
            var current = Shell.Current?.CurrentState?.Location?.ToString() ?? "";

            if (current.Contains("RosaryMeditations", StringComparison.OrdinalIgnoreCase))
                return;

            await Shell.Current.GoToAsync("RosaryMeditations");
        }

        private async void Prayers_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("Prayers");
        }

        private async void Retreat_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("Retreat");
        }
        private async void OrderMass_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("OrderMass");
        }
        private async void News_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("News");
        }

        private async void Login_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("Login",animate: true);
        }
    }
}
