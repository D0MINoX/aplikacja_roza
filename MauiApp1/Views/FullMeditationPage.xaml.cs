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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUI();
    }

    private void UpdateUI()
    {
        
        if (FullMeditation != null && !string.IsNullOrEmpty(_meditationText))
        {
            FullMeditation.Text = _meditationText;
        }
    }
    private async void CompletedTapped(object sender, TappedEventArgs e)
    {
        if (Application.Current.Resources.TryGetValue("Primary", out var color))
        {
            Complete.Background = (Color)color;
        }

    }
}