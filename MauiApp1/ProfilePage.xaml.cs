namespace MauiApp1;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MauiApp1.Models;
using Microsoft.Maui.Controls.Shapes;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    public ProfilePage(AuthService authService,RosaryService rosaryService)
	{
		InitializeComponent();
        _authService = authService;
        _rosaryService = rosaryService;
        DecodeToken(_authService.Token);
        RosariesShow();
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool IsLoggedout = await _authService.Logout();
        if (IsLoggedout)
        {
            await DisplayAlertAsync("Informacja", "Konto zostało pomyślnie wylogowane", "OK");
        }
        else
        {
            await DisplayAlertAsync("BŁĄD", "Wylogowanie nie powiodło się, spróbuj ponownie", "OK");
        }
        await Shell.Current.GoToAsync("//Home");
    }
    public void DecodeToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

      
        var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
        string userName = nameClaim?.Value ?? "Brak Imienia";

        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        string userRole = roleClaim?.Value ?? "Brak Roli";

        List<string> roles = ["admin", "główny zeletor","zelator","Członek rózy"];

        Name.Text ="Witaj "+ userName;
        Role.Text = "Rola:"+roles[int.Parse(userRole)];
       
      
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
            RosariesContainer.Children.Clear(); // Czyścimy listę
            if (rosaryInfos == null || rosaryInfos.Count == 0)
            {

                RosariesContainer.Children.Add(CreateJoinButton());
            }
            else
            {
                foreach (var rosary in rosaryInfos)
                {
                    try
                    {
                        var border = CreateRosaryCard(rosary.Name);
                        RosariesContainer.Children.Add(border);
                    }
                    catch (Exception ex)
                    {
                        // Debugowanie, jeśli zasób "Kafelki" nadal robi problem
                        System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                    }
                }
            }
        });

        }
       
    }
    private Border CreateRosaryCard(string rosary)
{

    var border = new Border
    {
        Padding = new Thickness(15),
        BackgroundColor = Colors.YellowGreen,
        StrokeShape = new RoundRectangle { CornerRadius = 10 },
        Margin = new Thickness(0, 5)
    };

    
    var label = new Label
    {
        Text = rosary,
        TextColor = Colors.White,
        FontAttributes = FontAttributes.Bold,
        FontSize = 18
    };
    border.Content = label;
    return border;
}
    private async void MyRosary_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("MyRosaryGroup");

    }
    private async Task RosaryJoin_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RosaryJoin");
    }
    private Border CreateJoinButton()
    {
        var border = new Border
        {
            Padding = new Thickness(15),
            BackgroundColor = Colors.YellowGreen,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Margin = new Thickness(0, 5),
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            await Shell.Current.GoToAsync("JoinRosary");
        };

        border.GestureRecognizers.Add(tapGesture);
        border.Content = new Label
        {
            Text = "Dołącz do róży",
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18
        };

        return border;
    }
}