using CommunityToolkit.Maui.Views;

namespace MauiApp1.Components;

public partial class PickerPopup : Popup
{
    private bool _isInitializing;
    string name;

    public PickerPopup(List<string> val, string name)
	{
        this.name = name;
        GenerateRadios(val);
		InitializeComponent();
	}


    public void GenerateRadios(List<string> val)
    {
        _isInitializing = true;


        RadioPicker.BatchBegin();
        RadioPicker.Children.Clear();
        foreach (var item in val)
        {
            var radio = new RadioButton
            {
                Content = item,
                Value = item,
            };

            radio.CheckedChanged += Radio_CheckedChanged;
            RadioPicker.Children.Add(radio);
        }
        RadioPicker.BatchCommit();

        string selected = "";
        if (val[0] == "Radosne")
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
        else if (val.Count > 0)
            RadioButtonGroup.SetSelectedValue(RadioPicker, val[0]);

        _isInitializing = false;
    }

    private void Radio_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_isInitializing || !e.Value)
            return;

        var radio = (RadioButton)sender;
        string val = radio.Value?.ToString();

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

        CloseAsync();
    }
}