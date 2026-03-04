namespace MauiApp1;


public partial class FullMeditationPage : ContentPage, IQueryAttributable 
{
    private string _meditationText;

    public FullMeditationPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("MeditationContent"))
        {
         
            _meditationText = query["MeditationContent"] as string;

     
            if (FullMeditation != null)
            {
                FullMeditation.Text = _meditationText;
            }
        }
    }

    private async void CompletedTapped(object sender, TappedEventArgs e)
    {
        Complete.Background = (Color)Application.Current.Resources["Kafelki"];

    }
}