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

        RadioPicker.BatchBegin();
        RadioPicker.Children.Clear();

        var firstRadio = new RadioButton
        {
            Content = "Nie wybieraj żadnej parafi",
            Value = 0,
        };

        firstRadio.CheckedChanged += Radio_CheckedChanged;
        RadioPicker.Children.Add(firstRadio);

        foreach (var item in lista)
        {
            var radio = new RadioButton
            {
                Content = item.Name,
                Value = item.Id,
            };

            radio.CheckedChanged += Radio_CheckedChanged;
            RadioPicker.Children.Add(radio);
        }
        RadioPicker.BatchCommit();

        PickerLabel.Text = "Wybierz Swoją parafie";
        int selected = Preferences.Get("ParishId", 0);
        RadioButtonGroup.SetSelectedValue(RadioPicker, selected);

        _isInitializing = false;
    }

    private void Radio_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_isInitializing || !e.Value)
            return;

        var radio = (RadioButton)sender;
        int parishId = (int)radio.Value;
        string parishName = radio.Content?.ToString();

        Preferences.Set("ParishId", parishId);
        Preferences.Set("ParishName", parishName);

        CloseAsync();
    }
}