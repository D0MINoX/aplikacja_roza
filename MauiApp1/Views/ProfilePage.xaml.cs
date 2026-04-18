namespace MauiApp1;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
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
    private bool isParish = false;
    private int UserId { get; set; }
    private string UserName { get; set; }
    private string UserSurname { get; set; }
    private string UserEmail { get; set; }
    private int UserRole { get; set; }
    private int Parish {  get; set; }
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
        if (UserId == 0) DecodeToken(_authService.Token);

        if (UserId > 0)
        {
            var response = await _parishService.GetUserParish(UserId);
            if (response.isSuccess)
            {
                
                ParishLabel.Text = response.Data.Name;
                Parish = response.Data.Id;
                if (Parish == -1)
                    isParish = false;
                else
                    isParish = true;

            }
            else
            {
                ParishLabel.Text = response.ErrorMessage;
            }

            RosariesShow();
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
    public async Task DecodeToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);


        var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
        string username = nameClaim?.Value ?? "Brak Imienia";

        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        string userRole = roleClaim?.Value ?? "Nie jesteś członkiem róży";
        var emailClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email);
        UserEmail = emailClaim?.Value ?? "Brak adresu Email";
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
        if (int.TryParse(IdClaim?.Value, out int id))
        {
            this.UserId = id; 
        }
        List<string> roles = ["Rola: admin", "Rola: główny zeletor", "Zelator: ", "Członek rózy: "];

        Name.Text = "Witaj " + username;
        UserName = username.Split(" ")[0];
        UserSurname = username.Split(" ")[1];
        UserRole = int.Parse(userRole);
        Role.Text = roles[UserRole];
        if (UserId == 1)
        {
            adminBtn.IsVisible = true;
        }
        
#if WINDOWS || MACCATALYST
        if (UserRole < 3 || UserId==1)
        {
            adminBtn.IsVisible = true;
        }
#endif
        }

    private async void RosariesShow()
    {
        List<RosaryInfo> rosaryInfos = await _rosaryService.GetUserRosariesAsync(UserId);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (rosaryInfos == null || rosaryInfos.Count == 0)
            {
                Role.Text = "Nie należysz do żadnej róży";

                RosariesContainer.IsVisible = true;
            }
            else
            {
                List<string> roles = ["Rola: admin", "Rola: główny zeletor", "Zelator: ", "Członek rózy: "];
                Role.Text = roles[UserRole]+" "+rosaryInfos[0].Name;
            }
        });
    }

    private async void JoinBtn_Clicked(object sender, EventArgs e)
    {
        if (isParish)
        {
            var navigationParameter = new Dictionary<string, object> { { "UserId", UserId }, { "Parish", Parish } };
            await Shell.Current.GoToAsync("JoinRosary", navigationParameter);
        }
        else
        {
            var navigationParameter = new Dictionary<string, object> { { "UserId", UserId } };
            await Shell.Current.GoToAsync("SelectParish", navigationParameter);
        }
    }

    private void AdminBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("AdminPage");
    }

    private async void UserEditTapped(object sender, EventArgs e)
    {
        var popup = new EditUserPopup(UserName, UserSurname, UserEmail);
        var result = await this.ShowPopupAsync<EditUserResult>(popup, new PopupOptions
        {
            Shape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10),
                StrokeThickness = 0
            },
            Shadow = null
        });


        if (result?.Result is EditUserResult data && !result.WasDismissedByTappingOutsideOfPopup)
        {
            var updateResult = await _authService.UpdateUserAsync(UserId, data.Name, data.Surname, data.Email);

            if (updateResult)
            {
                await DisplayAlertAsync("Sukces", "Dane użytkownika zostały zapisane.", "OK");
                DecodeToken(_authService.Token);
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Wystąpił błąd podczas zmiany danych spróbuj ponownie", "OK");
            }
        }
    }

    private async void PasswordEditTapped(object sender, EventArgs e)
    {
        var result =  await this.ShowPopupAsync<EditPasswordResult>(new EditPasswordPopup(), new PopupOptions
        {
            Shape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10),
                StrokeThickness = 0
            },
            Shadow = null
        });
        if(result?.Result is EditPasswordResult data && !result.WasDismissedByTappingOutsideOfPopup)
        {
            var updateResult = await _authService.UpdatePasswordAsync(UserId,result.Result.OldPassword,result.Result.NewPassword);
            if (updateResult)
            {
                await DisplayAlertAsync("Sukces", "Dane użytkownika zostały zapisane.", "OK");
                
            }
            else
            {
                await DisplayAlertAsync("Błąd", "Wystąpił błąd podczas zmiany danych spróbuj ponownie", "OK");
            }
        } 
    }
    private async void DeleteTapped(object sender, EventArgs e) {
        string password = PasswordEntry.Text;
        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlertAsync("BŁĄD", "Hasło nie zostało wpisane", "OK");
        }
        else
        {
            var result = await _authService.deleteUser(UserId, password);
            if (result) 
                {
                    await DisplayAlertAsync("Sukces", "Dane użytkownika zostały usunięte.", "OK");
                    await Shell.Current.GoToAsync("//Home");
            }
            else
                {
                    await DisplayAlertAsync("Błąd", "Wystąpił błąd podczas usuwania danych spróbuj ponownie", "OK");
                }
            }
    
    }
}