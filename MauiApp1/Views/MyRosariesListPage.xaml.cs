using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls.Shapes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class MyRosariesListPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly RosaryService _rosaryService;
    private readonly MessagesService _messagesService;
    private readonly ParishService _parishService;
    private HashSet<int> _selectedRosaries;
    private HashSet<int> _allRosaries;
    private bool _isSend = false;
    private bool _isLoading = false;
    public MyRosariesListPage(AuthService authService, RosaryService rosaryService, MessagesService messagesService, ParishService parishService)
	{
        InitializeComponent();
        _authService = authService;
        _rosaryService = rosaryService;
        _messagesService = messagesService;
        _parishService = parishService;
        _selectedRosaries = new HashSet<int>();
        _allRosaries = new HashSet<int>();
        RosariesShow();
    }

    private async void RosariesShow()
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);
        var IdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);

        if (IdClaim != null && int.TryParse(IdClaim.Value, out int Id))
        {
            List<RosaryInfo> rosaryInfos = new List<RosaryInfo>();
            var response = await _parishService.GetUserParish(Id);
            if (response.isSuccess)
            {
                if (response.Data.Id!=null)
                {
                    rosaryInfos = await _rosaryService.GetAvailableRosariesAsync(response.Data.Id);
                }
            }
            if (rosaryInfos.Count<1)
            {
                rosaryInfos = await _rosaryService.GetAllRosariesAsync();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                RosariesContainer.BatchBegin();
                try
                {
                    RosariesContainer.Children.Clear();
                    foreach (var rosary in rosaryInfos)
                    {
                        try
                        {
                            var border = CreateRosaryCard(rosary.Name, rosary.Id);
                            RosariesContainer.Children.Add(border);
                            _allRosaries.Add(rosary.Id);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                        }
                    }
                }
                finally
                {
                    RosariesContainer.BatchCommit();
                }
            });
        }
    }

    private Border CreateRosaryCard(string rosary, int rosaryId)
    {
        var colorPrimary = (Color)Application.Current.Resources["Primary"];
        var colorSecondary = (Color)Application.Current.Resources["Secondary"];
        var colorOutline = (Color)Application.Current.Resources["Accent"];

        var border = new Border
        {
            Style = (Style)Application.Current.Resources["ListElement"],
        };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            if (_isSend)
            {
                Border tappedBorder = s as Border;
                if (_selectedRosaries.Contains(rosaryId))
                {
                    _selectedRosaries.Remove(rosaryId);
                    tappedBorder.BackgroundColor = colorPrimary;
                }
                else
                {
                    _selectedRosaries.Add(rosaryId);
                    tappedBorder.BackgroundColor = colorSecondary;
                }
            }
            else
            {
                var navigationParameter = new Dictionary<string, object>{{ "RosaryId", rosaryId.ToString() }};
                await Shell.Current.GoToAsync("MyRosaryGroup", navigationParameter);
            }
        };
        border.GestureRecognizers.Add(tapGesture);
        var label = new Label
        {
            Text = rosary,
        };
        border.Content = label;
        return border;
    }

    private void Send_Tapped(object sender, EventArgs e)
    {
        Color color;
        if (_isSend)
        {
            color = (Color)Application.Current.Resources["Primary"];
            ClearSelected();
        }
        else
            color = (Color)Application.Current.Resources["Secondary"];

        _isSend = !_isSend;
        SendOptionsGrid.IsVisible = !SendOptionsGrid.IsVisible;
        SendMessage.BackgroundColor = color;
    }

    private async void ConfirmSend_Tapped(object sender, EventArgs e)
    {
        if (_isLoading) return;
        _isLoading = true;

        SendMessages(_selectedRosaries);
        ClearSelected();

        _isLoading = false;
    }

    private async void SendToAll_Tapped(object sender, EventArgs e)
    {
        if (_isLoading) return;
        _isLoading = true;

        SendMessages(_allRosaries);
        ClearSelected();

        _isLoading = false;
    }

    private async void SendMessages(HashSet<int> rosaryIds)
    {
        var popup = new NewMessagePopup(new RosaryMessage());
        var result = await this.ShowPopupAsync<RosaryMessage>(popup);

        if (!result.WasDismissedByTappingOutsideOfPopup)
        {
            RosaryMessage message = result.Result;
            if (message != null && rosaryIds.Count > 0)
            {
                if (string.IsNullOrEmpty(_authService.Token)) return;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(_authService.Token);

                var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
                string userName = nameClaim?.Value ?? "Brak Imienia";
                message.AuthorName = userName;
                message.CreatedAt = DateTime.Now;

                foreach (var rosaryId in rosaryIds)
                {
                    message.RosaryId = rosaryId;
                    await _messagesService.NewMessageAsync(message);
                }
                await DisplayAlertAsync("info", "wiadomości zostały wysłane", "OK");
            }
        }
    }

    private void ClearSelected()
    {
        foreach (var child in RosariesContainer.Children)
        {
            if (child is Border border)
            {
                border.BackgroundColor = (Color)Application.Current.Resources["Primary"];
            }
        }
    }
}