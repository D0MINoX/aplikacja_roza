using CommunityToolkit.Maui.Views;
using MauiApp1.Models;

namespace MauiApp1.Components;

public partial class ParishPickerPopup : Popup
{
    bool _isInitializing;

    public ParishPickerPopup(List<Parish> val)
	{
		InitializeComponent();
        GenerateForParish(val);
	}

    public void GenerateForParish(List<Parish> lista)
    {
        _isInitializing = true;

        int selected = Preferences.Get("ParishId", 0);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Picker.BatchBegin();
            try
            {
                Picker.Children.Clear();

                Parish defaultOption = new Parish { Id = 0, Name = "Nie wybieraj żadnej parafi" };
                var defaultBorder = CreateOption(defaultOption);
                if (selected==0)
                {
                    defaultBorder.BackgroundColor = (Color)Application.Current.Resources["TertiaryFaded"];
                }
                Picker.Children.Add(defaultBorder);

                foreach (Parish item in lista)
                {
                    try
                    {
                        Border border = CreateOption(item);
                        if (item.Id == selected)
                        {
                            border.BackgroundColor = (Color)Application.Current.Resources["TertiaryFaded"];
                        }
                        Picker.Children.Add(border);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd tworzenia kafelka: {ex.Message}");
                    }
                }
            }
            finally
            {
                Picker.BatchCommit();
            }
        });

        _isInitializing = false;
    }

    private Border CreateOption(Parish item)
    {
        var colorPrimary = (Color)Application.Current.Resources["Primary"];
        var colorSelected = (Color)Application.Current.Resources["TertiaryFaded"];
        var borderStyle = (Style)Application.Current.Resources["ListElement"];

        var border = new Border
        {
            Style = borderStyle
        };

        var tapGesture = new TapGestureRecognizer { CommandParameter = item };
        tapGesture.Tapped += (s, e) =>
        {
            if(_isInitializing) return;

            foreach (var child in Picker.Children)
            {
                if (child is Border b) b.BackgroundColor = colorPrimary;
            }

            border.BackgroundColor = colorSelected;

            Preferences.Set("ParishId", item.Id);
            Preferences.Set("ParishName", item.Name);

            CloseAsync();
        };
        border.GestureRecognizers.Add(tapGesture);

        var label = new Label
        {
            Text = item.Name
        };
        border.Content = label;

        return border;
    }
}