using MauiApp1.Resources.Styles;

public static class ThemeManager
{
    public static void SetTheme(string themeName)
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

            case "Swiatla":
                merged.Add(new SwiatlaTheme());
                break;

            case "Radosne":
            default:
                merged.Add(new RadosneTheme());
                break;
        }
    }
}

