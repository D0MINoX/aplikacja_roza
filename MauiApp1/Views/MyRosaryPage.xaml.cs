using System.Diagnostics;

namespace MauiApp1;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class MyRosaryPage : ContentPage
{
    private string _rosaryId;
    public string RosaryId
    {
        get => _rosaryId;
        set
        {
            _rosaryId = value;
        }
    }
    public MyRosaryPage()
    {
        InitializeComponent();
    }



    private async void Messages_Tapped(object sender, TappedEventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                                {
                                    { "RosaryId", RosaryId }
                                };
        await Shell.Current.GoToAsync("Messages", navigationParameter);
    }

    private async void ViewGroups_Tapped(object sender, TappedEventArgs e)
    {
        // await Shell.Current.GoToAsync("ViewGroups");
    }

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Login");
    }
}