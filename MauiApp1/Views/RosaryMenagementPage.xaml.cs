using MauiApp1.Models;
using MauiApp1.Services;
    
namespace MauiApp1.Views;

[QueryProperty(nameof(RosaryId), "RosaryId")]
public partial class RosaryMenagementPage : ContentPage
{
    private readonly AdminService _adminService;
    public int RosaryId { get; set; }
    public RosaryMenagementPage(AdminService adminService)
    {
        _adminService = adminService;
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