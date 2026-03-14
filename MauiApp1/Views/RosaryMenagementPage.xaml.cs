namespace MauiApp1.Views;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class RosaryMenagementPage : ContentPage
{

    public int RosaryId { get; set; }
    public RosaryMenagementPage()
    {

        InitializeComponent();
    }

    private async void UserVerification_Tapped(object sender, TappedEventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "RosaryId", RosaryId }
        };
        await Shell.Current.GoToAsync("UserVerification", navigationParameter);

    }
}