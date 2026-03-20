
using MauiApp1.Models;
using MauiApp1.Services;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiApp1.Views;

public partial class MessagesPage : ContentPage, IQueryAttributable
{
    private int RosaryId { get; set; }
    public MessagesService _messagesService;
    public AuthService _authService;
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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateData();
        if (string.IsNullOrEmpty(_authService.Token)) return;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);


        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        int userRole = int.Parse(roleClaim?.Value ?? "5");
        if (userRole > 2)
        {
            FabButton.IsVisible = false;
        }
    }
    private async void OnFabClicked(object sender, EventArgs e)
    {
      
        OverlayBackground.IsVisible = true;
        AddMessagePanel.IsVisible = true;
        FabButton.IsVisible = false; 

       
        await Task.WhenAll(
            OverlayBackground.FadeToAsync(1, 200),
            AddMessagePanel.FadeToAsync(1, 200, Easing.SpringOut),
            AddMessagePanel.ScaleToAsync(1, 200, Easing.SpringOut)
        );

       
        NewTitleEntry.Focus();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
       
        NewTitleEntry.Unfocus();
        NewBodyEditor.Unfocus();

        await Task.WhenAll(
            OverlayBackground.FadeToAsync(0, 150),
            AddMessagePanel.FadeToAsync(0, 150),
            AddMessagePanel.ScaleToAsync(0.8, 150)
        );

        OverlayBackground.IsVisible = false;
        AddMessagePanel.IsVisible = false;
        FabButton.IsVisible = true; 
    }
    private async void OnSendMessageClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_authService.Token)) return;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(_authService.Token);


        var nameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name);
        string userName = nameClaim?.Value ?? "Brak Imienia";
        RosaryMessage message = new RosaryMessage();
        message.RosaryId = RosaryId;
        message.AuthorName = userName;
        message.MessageTitle = NewTitleEntry.Text;
        message.MessageBody = NewBodyEditor.Text;
        message.CreatedAt = DateTime.Now;
        var success = await _messagesService.NewMessageAsync(message);
        if (success)
        {
            OnCancelClicked(sender, e);
            UpdateData();
        }
    }
    private async void UpdateData()
    {

        var message = await _messagesService.GetMessagesAsync(RosaryId);
        if (message.isSuccess)
        {
            MessagesList.ItemsSource = message.Data;
        }
      
    }
}