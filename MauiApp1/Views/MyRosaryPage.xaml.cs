using MauiApp1.Services;

using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace MauiApp1;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class MyRosaryPage : ContentPage
{
    public readonly RosaryService _rosaryService;
    private string _rosaryId;
    public string RosaryId
    {
        get => _rosaryId;
        set
        {
            _rosaryId = value;
        }
    }
    public MyRosaryPage(RosaryService rosaryService)
    {
        InitializeComponent();
        _rosaryService = rosaryService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var response = await _rosaryService.GetNameAsync(int.Parse(RosaryId));
        var data = JObject.Parse(response);
        string name = data["name"].ToString();
        rosaryName.Text = name;
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