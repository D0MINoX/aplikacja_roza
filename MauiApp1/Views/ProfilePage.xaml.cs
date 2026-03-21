namespace MauiApp1;

using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls.Shapes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    private readonly ParishService _parishService;
    private int UserId { get; set; }
    public ProfilePage(AuthService authService,ParishService parishService, RosaryService rosaryService)
    {
        InitializeComponent();
        _authService = authService;
        _rosaryService = rosaryService;
        _parishService = parishService;
        DecodeToken(_authService.Token);
        //RosariesShow();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var response = await _parishService.GetUserParish(UserId);
        if (response.isSuccess)
        {
            ParishLabel.Text = response.Data.Name;
        }
        else
        {
            ParishLabel.Text = response.ErrorMessage;
        }
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
        string userRole = roleClaim?.Value ?? "Nie jesteś członkiem róży";
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        int.TryParse(IdClaim.Value, out int Id);
        UserId = Id;
        List<string> roles = ["Rola: admin", "Rola: główny zeletor", "Zelator: ", "Członek rózy: "];

        Name.Text = "Witaj " + userName;
        Role.Text = roles[int.Parse(userRole)];
        if (userRole == "2" || userRole == "3")
        {
            RosariesShow();
        }
        #if WINDOWS || MACCATALYST
        if (int.Parse(userRole) < 3)
        {
            adminBtn.IsVisible = true;
            var adminBTN = CreateAdminButton();
            adminBtn.Children.Add(adminBTN);
        }
        #endif
    }

    private async void RosariesShow()
    {
        
            List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(UserId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RosariesContainer.Children.Clear(); // Czyścimy listę
                if (rosaryInfos == null || rosaryInfos.Count == 0)
                {
                    Role.Text += "Nie należysz do żadnej róży";
                    RosariesContainer.IsVisible = true;
                    RosariesContainer.Children.Add(CreateJoinButton(UserId));
                }
                else
                {
                    Role.Text += rosaryInfos[0].Name;
                }
            });
        
    }
    private Border CreateJoinButton(int UserId)
    {
        var borderStyle = (Style)Application.Current.Resources["MenuOption"];
        var labelStyle = (Style)Application.Current.Resources["OptionLabel"];

        var border = new Border
        {
            Style = borderStyle
        };

        var navigationParameter = new Dictionary<string, object> { { "UserId", UserId } };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            await Shell.Current.GoToAsync("JoinRosary", navigationParameter);
        };

        border.GestureRecognizers.Add(tapGesture);
        border.Content = new Label
        {
            Text = "Dołącz do róży",
            Style = labelStyle
        };

        return border;
    }
    private Border CreateAdminButton()
    {
        var borderStyle = (Style)Application.Current.Resources["MenuOption"];
        var labelStyle = (Style)Application.Current.Resources["OptionLabel"];

        var border = new Border
        {
            Style = borderStyle
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {

            await Shell.Current.GoToAsync("AdminPage");
        };

        border.GestureRecognizers.Add(tapGesture);
        border.Content = new Label
        {
            Style = labelStyle
        };

        return border;

    }

    private void EditTapped(object sender, EventArgs e)
    {
        
        if (sender==NameBtn)
        {
            var label = (Label)NameBtn.Content;
            if (label.Text=="Edytuj")
            {
                label.Text = "Zatwierdź";

            }
        }
    }
}