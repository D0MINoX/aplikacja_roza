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
           

            await Shell.Current.GoToAsync("MyRosaryGroup");
        }
        private async void RosaryMeditations_Tapped(object sender, TappedEventArgs e)
        {
            

            await Shell.Current.GoToAsync("RosaryMeditations");
        }


    }
}
