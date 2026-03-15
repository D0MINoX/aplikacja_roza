using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls.Shapes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class AdminRosariesPage : ContentPage
{

    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    public AdminRosariesPage(AuthService authService, RosaryService rosaryService)
    {

        InitializeComponent();
        _authService = authService;
        _rosaryService = rosaryService;
        RosariesShow();
    }
    private async void RosariesShow()
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        if (IdClaim != null && int.TryParse(IdClaim.Value, out int Id))
        {
            List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(Id);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RosariesContainer.Children.Clear();

                foreach (var rosary in rosaryInfos)
                {
                    try
                    {
                        var border = CreateRosaryCard(rosary.Name, rosary.Id);
                        RosariesContainer.Children.Add(border);
                    }
                    catch (Exception ex)
                    {

                        System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                    }
                }

            });

        }

    }
    private Border CreateRosaryCard(string rosary, int rosaryId)
    {
        var colorPrimary = (Color)Application.Current.Resources["Primary"];
        var colorMenu = (Color)Application.Current.Resources["Secondary"];
        var colorOutline = (Color)Application.Current.Resources["Accent"];
        var colorText = (Color)Application.Current.Resources["Text"];

        var border = new Border
        {
            Padding = new Thickness(15),
            BackgroundColor = colorPrimary,
            Stroke = colorOutline,
            StrokeThickness = 2,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Margin = new Thickness(0, 5)
        };
        border.BackgroundColor = colorMenu;
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {

            var navigationParameter = new Dictionary<string, object>
        {
            { "RosaryId", rosaryId }
        };
            await Shell.Current.GoToAsync("RosaryMenagement", navigationParameter);
        };
        border.GestureRecognizers.Add(tapGesture);
        var label = new Label
        {
            Text = rosary,
            TextColor = colorText,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18
        };
        border.Content = label;
        return border;
    }
}