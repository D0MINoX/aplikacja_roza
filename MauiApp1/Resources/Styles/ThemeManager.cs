using MauiApp1.Resources.Styles;

public static class ThemeManager
{
    private const string ThemeKey = "app_theme";
    private const string MainThemeKey = "app_main_theme";
    public static void ApplySavedTheme()
    {
        string themeName = Preferences.Get(ThemeKey, "Radosne");
        bool mainThemeName = Preferences.Get(MainThemeKey, false);
        ApplyTheme(themeName);
        ApplyMainTheme(mainThemeName);

    }

    public static void SetTheme(string themeName)
    {
        Preferences.Set(ThemeKey, themeName);
        ApplyTheme(themeName);
    }
    public static void ApplyTheme(string themeName)
    {
        var merged = Application.Current!.Resources.MergedDictionaries;

        var currentTheme = merged.FirstOrDefault(d =>
            d is RadosneTheme ||
            d is BolesneTheme ||
            d is ChwalebneTheme ||
            d is SwiatlaTheme);

        if (currentTheme != null)
            merged.Remove(currentTheme);

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

    public static void SetMainTheme()
    {
        bool mainThemeName = Preferences.Get(MainThemeKey, false);
        Preferences.Set(MainThemeKey, !mainThemeName);
        ApplyMainTheme(!mainThemeName);
    }
    public static void ApplyMainTheme(bool mainThemeName)
    {
        var merged = Application.Current!.Resources.MergedDictionaries;

        var currentTheme = merged.FirstOrDefault(d =>
            d is LightTheme ||
            d is DarkTheme);

        if (currentTheme != null)
            merged.Remove(currentTheme);

        switch (mainThemeName)
        {
            case true:
                merged.Add(new DarkTheme());
                break;

            case false:
            default:
                merged.Add(new LightTheme());
                break;
        }
    }
}

