using CommunityToolkit.Maui.Views;
using MauiApp1.Models;

namespace MauiApp1.Components;

public partial class NewMessagePopup : Popup<RosaryMessage>
{
	private RosaryMessage _message;
	public NewMessagePopup(RosaryMessage message)
	{
		InitializeComponent();
		_message = message;
	}

	public async void Cancel_Tapped(object sender, EventArgs e)
	{
		await this.CloseAsync(null);
    }

	public async void Send_Tapped(object sender, EventArgs e)
	{
		_message.MessageTitle = TitleEntry.Text;
		_message.MessageBody = BodyEditor.Text;
		await this.CloseAsync(_message);
    }
}