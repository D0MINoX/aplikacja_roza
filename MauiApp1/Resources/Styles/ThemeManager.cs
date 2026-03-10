using MauiApp1.Resources.Styles;
using Microsoft.Maui.Storage;

public static class ThemeManager
{
    private const string ThemeKey = "app_theme";
    public static void ApplySavedTheme()
    {
        string themeName = Preferences.Get(ThemeKey, "Radosne");
        ApplyTheme(themeName);
    }

    public static void SetTheme(string themeName)
    {
        Preferences.Set(ThemeKey, themeName);
        ApplyTheme(themeName);
    }
    public static void ApplyTheme(string themeName)
    {
        var merged = Application.Current!.Resources.MergedDictionaries;

        merged.Clear();

        switch (themeName)
        {
            case "Chwalebne":
                merged.Add(new ChwalebneTheme());
                break;

            case "Bolesne":
                merged.Add(new BolesneTheme());
                break;

            case "Światła":
                merged.Add(new SwiatlaTheme());
                break;

            case "Radosne":
            default:
                merged.Add(new RadosneTheme());
                break;
        }
    }
}

