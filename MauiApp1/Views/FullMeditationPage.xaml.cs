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
            int date = Preferences.Default.Get("LastDate", 1);
            DayLabel.Text = "Dzień " + date.ToString();
            FullMeditation.Text = _meditationText;
        }
    }
    private async void CompletedTapped(object sender, TappedEventArgs e)
    {
        Color? color = Complete.BackgroundColor;
        float newAlpha = color.Alpha < 1f ? 1f : 0.5f;
        Complete.BackgroundColor = color.WithAlpha(newAlpha);
    }

    private async void BackTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}