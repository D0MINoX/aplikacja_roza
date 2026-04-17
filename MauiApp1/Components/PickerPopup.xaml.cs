using CommunityToolkit.Maui.Views;
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

        RadioPicker.BatchBegin();
        RadioPicker.Children.Clear();
        foreach (var item in lista)
        {
            var btn = new Button
            {
                Text = item,
                // Ustawiamy przezroczyste tło, żeby przycisk wyglądał jak zwykły tekst/lista
                BackgroundColor = (Color)Application.Current!.Resources["Primary"],
                // Dostosowanie koloru czcionki (jeśli masz np. ciemny motyw)
                TextColor = (Color)Application.Current!.Resources["Text"] ,
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(10, 5),
                FontSize = 16
            };

            // Kiedy przycisk zostanie kliknięty, wywołujemy naszą nową metodę
            btn.Clicked += (sender, e) => OptionSelected(item);

            RadioPicker.Children.Add(btn);
        }
        RadioPicker.BatchCommit();

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

        _isInitializing = false;
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