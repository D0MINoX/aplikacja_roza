namespace MauiApp1;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    public ProfilePage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
        DecodeToken(_authService.Token);
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
        var rosaryClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "rosary" || c.Type == "Rosary");
        string rosaryRole = rosaryClaim?.Value ?? "Brak Róży";

        List<string> roles = ["admin", "główny zeletor","zelator","Członek rózy"];

        Name.Text ="Witaj "+ userName;
        Role.Text = "Rola:"+roles[int.Parse(userRole)];
       
         switch (int.Parse(rosaryRole))
         {
            case -1:
                Rosary.Text = "Dołącz do róży";
                break;
            case 0:
                Rosary.Text = "lista róż";
                break;
            default:
                Rosary.Text = "Moja róża: " + rosaryRole;
                break;
            }
      
    }

    private async void MyRosary_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("MyRosaryGroup");

    }
    private async Task RosaryJoin_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RosaryJoin");
    }
}