namespace MauiApp1;

public partial class RosaryMeditationsPage : ContentPage
{
    public DateTime date;
    public MeditationsService _meditationService;
	public RosaryMeditationsPage(MeditationsService meditationService)
	{
		InitializeComponent();
        _meditationService = meditationService;
        date = DateTime.Now;
        UpdateDate();
    }

    private async void UpdateDate()
    {
        DateLabel.Text = date.ToString("dd-MM");

        string description = await _meditationService.GetOnlyDescription(date, "Zwiastowanie Najświętszej Maryi Pannie");
        MeditationLabel.Text = description ?? "Brak rozważania";
    }

    private async void PreviousTapped(object sender, EventArgs e)
    {
        date = date.AddDays(-1);
        UpdateDate();
    }

    private async void NextTapped(object sender, EventArgs e)
    {
        date = date.AddDays(1);
        UpdateDate();
    }

    private async void MeditationTapped(object sender, EventArgs e)
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
}