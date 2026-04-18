using CommunityToolkit.Maui.Views;
using MauiApp1.Models;
using MauiApp1.Views;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiApp1.Components;

public partial class PickerPopup : Popup
{
    private bool _isInitializing;
    private string name;

    public PickerPopup(List<string> val, string name)
	{
        this.name = name;
		InitializeComponent();
        GenerateForMeditions(val);
	}

    public void GenerateForMeditions(List<string> lista)
    {
        _isInitializing = true;

        string selected = "";
        if (name == "Group")
        {
            PickerLabel.Text = "Wybierz część różańca";
            selected = Preferences.Default.Get("LastGroup", "");
        }
        else
        {
            PickerLabel.Text = "Wybierz tajemnicę";
            selected = Preferences.Default.Get("LastMystery", "");
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Picker.BatchBegin();
            try
            {
                Picker.Children.Clear();

                foreach (var item in lista)
                {
                    try
                    {
                        Border border = CreateOption(item);
                        if (item == selected)
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

    private Border CreateOption(string item)
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
            foreach (var child in Picker.Children)
            {
                if (child is Border b) b.BackgroundColor = colorPrimary;
            }

            border.BackgroundColor = colorSelected;
            OptionSelected(item);
        };
        border.GestureRecognizers.Add(tapGesture);

        var label = new Label
        {
            Text = item
        };
        border.Content = label;

        return border;
    }

    private void OptionSelected(string val)
    {
        if (name == "Group")
        {
            Preferences.Default.Set("LastGroup", val);
            switch (val)
            {
                case "Światła":
                    Preferences.Default.Set("LastMystery", "Chrzest Pana Jezusa w Jordanie");
                    break;
                case "Bolesne":
                    Preferences.Default.Set("LastMystery", "Modlitwa Pana Jezusa w Ogrójcu");
                    break;
                case "Chwalebne":
                    Preferences.Default.Set("LastMystery", "Zmartwychwstanie Pana Jezusa");
                    break;
                case "Radosne":
                default:
                    Preferences.Default.Set("LastMystery", "Zwiastowanie Najświętszej Maryi Pannie");
                    break;
            }

            ThemeManager.SetTheme(val);
        }
        else
        {
            Preferences.Default.Set("LastMystery", val);
        }

        Preferences.Default.Set("LastDate", 1);
        CloseAsync(); // Zamyka popup
    }

}