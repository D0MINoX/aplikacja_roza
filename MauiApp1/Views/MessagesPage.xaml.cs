
using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;
using MauiApp1.Models;
using MauiApp1.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class MessagesPage : ContentPage, IQueryAttributable
{
    private int RosaryId { get; set; }
    public MessagesService _messagesService;
    public AuthService _authService;
    private bool _isLoading = false;

    public MessagesPage(MessagesService messagesService,AuthService authService)
	{
        _messagesService = messagesService;
        _authService = authService;
		InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("RosaryId"))
        {
             string _RosaryId = query["RosaryId"] as string;
            RosaryId = int.Parse(_RosaryId);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateData();
        FabButton.IsVisible = await _authService.CanUserSendSmsAsync();

    }

    private async void OnFabClicked(object sender, EventArgs e)
    {
        if (_isLoading) return;
        _isLoading = true;

        var popup = new NewMessagePopup(new RosaryMessage());
        var result = await this.ShowPopupAsync<RosaryMessage>(popup);

        if (!result.WasDismissedByTappingOutsideOfPopup)
        {
            RosaryMessage message = result.Result;
            if (message != null)
            {
                SendMessage(message);
            }
        }

        _isLoading = false;
    }

    private async void SendMessage(RosaryMessage message)
    {
        if (string.IsNullOrEmpty(_authService.Token)) return;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);

        var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
        string userName = nameClaim?.Value ?? "Brak Imienia";
        message.RosaryId = RosaryId;
        message.AuthorName = userName;
        message.CreatedAt = DateTime.Now;

        var success = await _messagesService.NewMessageAsync(message);
        
        if (success)
        {
            var externalPhones = await _messagesService.getExternalNumbers(RosaryId);
            if (externalPhones != null && externalPhones.Any())
            {
                try
                {
                    string[] recipients = externalPhones.ToArray();
                    var smsMessage = new SmsMessage(
                        $"{message.MessageTitle}: {message.MessageBody}",
                        recipients);

                    await Sms.Default.ComposeAsync(smsMessage);
                }
                catch (FeatureNotSupportedException)
                {
                    await DisplayAlertAsync("Błąd", "SMS nie jest obsługiwany na tym urządzeniu.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlertAsync("Błąd", $"Nie udało się wysłać SMS : {ex.Message}", "OK");
                }
            }
            UpdateData();
        }
    }

    private async void UpdateData()
    {
        var message = await _messagesService.GetMessagesAsync(RosaryId);
        if (message.isSuccess)
        {
            MessagesList.ItemsSource = message.Data;
            if (message.Data?.Count > 0)
            {
                MessagesList.ScrollTo(
                    message.Data.Count - 1,
                    position: ScrollToPosition.End,
                    animate: false);
            }
        }
    }
}