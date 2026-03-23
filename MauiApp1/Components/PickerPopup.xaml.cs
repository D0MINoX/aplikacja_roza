using CommunityToolkit.Maui.Views;

namespace MauiApp1.Components;

public partial class PickerPopup : Popup
{
    String name;
	public PickerPopup(string val)
	{
		InitializeComponent();
        name = val;
        GenerateRadios(val);
	}

    public void GenerateRadios(string val)
    {
        var items = val switch
        {
            "Group" => new List<string> 
            {
                "Radosne",
                "Światła",
                "Bolesne",
                "Chwalebne"
            },
            "Radosne" => new List<string>
        {
            "Zwiastowanie Najświętszej Maryi Pannie",
            "Nawiedzenie św. Elżbiety",
            "Narodzenie Pana Jezusa",
            "Ofiarowanie Pana Jezusa w świątyni",
            "Odnalezienie Pana Jezusa w świątyni"
        },
            "Światła" => new List<string>
        {
            "Chrzest Pana Jezusa w Jordanie",
            "Objawienie się Pana Jezusa w Kanie Galilejskiej",
            "Głoszenie Królestwa Bożego i wzywanie do nawrócenia",
            "Przemienienie na górze Tabor",
            "Ustanowienie Eucharystii"
        },
            "Bolesne" => new List<string>
        {
            "Modlitwa Pana Jezusa w Ogrójcu",
            "Biczowanie Pana Jezusa",
            "Cierniem ukoronowanie Pana Jezusa",
            "Dźwiganie krzyża na Kalwarię",
            "Ukrzyżowanie i śmierć Pana Jezusa"
        },
            "Chwalebne" => new List<string>
        {
            "Zmartwychwstanie Pana Jezusa",
            "Wniebowstąpienie Pana Jezusa",
            "Zesłanie Ducha Świętego",
            "Wniebowzięcie Najświętszej Maryi Panny",
            "Ukoronowanie Najświętszej Maryi Panny na Królową Nieba i Ziemi"
        },
            _ => new List<string>()
        };

        RadioPicker.Children.Clear();

        foreach (var item in items)
        {
            var radio = new RadioButton
            {
                Content = item,
                Value = item,
            };

            radio.CheckedChanged += Radio_CheckedChanged;
            RadioPicker.Children.Add(radio);
        }

        String selected = "";
        if (val == "Group")
        {
            PickerLabel.Text = "Wybierz część różańca";
            selected = Preferences.Default.Get("LastGroup","");
        }
        else
        {
            PickerLabel.Text = "Wybierz tajemnicę";
            selected = Preferences.Default.Get("LastMystery","");
        }

        if (!string.IsNullOrEmpty(selected))
            RadioButtonGroup.SetSelectedValue(RadioPicker, selected);
        else if (items.Count > 0)
            RadioButtonGroup.SetSelectedValue(RadioPicker, items[0]);
    }

    private void Radio_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        var radio = (RadioButton)sender;
        String val = radio.Value?.ToString();

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
    }
}