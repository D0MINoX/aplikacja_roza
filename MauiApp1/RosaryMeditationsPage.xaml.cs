namespace MauiApp1;

public partial class RosaryMeditationsPage : ContentPage
{
    public DateTime date;
	public RosaryMeditationsPage()
	{
		InitializeComponent();
        date = DateTime.Now;
        UpdateDate();
    }

    private void UpdateDate()
    {
        DateLabel.Text = date.ToString("dd-MM");
        //pobierane z bazy
        //MeditationLabel.Text = "";
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
        await Shell.Current.GoToAsync("FullMeditation");
    }

}