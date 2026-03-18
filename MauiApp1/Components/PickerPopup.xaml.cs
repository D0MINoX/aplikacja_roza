using CommunityToolkit.Maui.Views;

namespace MauiApp1.Components;

public partial class PickerPopup : Popup
{
	public PickerPopup()
	{
		InitializeComponent();
	}

	public void OptionTapped(object sender, EventArgs e)
	{
        if (sender is Border border)
        {
            var radioButton = border.GetVisualTreeDescendants().OfType<RadioButton>().FirstOrDefault();
            radioButton.IsChecked = !radioButton.IsChecked;
        }
    }
}