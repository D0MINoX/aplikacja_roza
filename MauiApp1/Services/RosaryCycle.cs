namespace MauiApp1.Services;

internal static class RosaryCycle
{
    private const string RestoreKey = "RestoreCalendarOnStart";
    private const string ResetDateKey = "RosaryCycleResetDate";
    private const string MysteryKey = "CompletedCalendarMystery";

    internal static string SavedMystery
    {
        get
        {
            // Migrate only a legacy choice with an explicitly completed day.
            if (!Preferences.Default.ContainsKey(MysteryKey))
                Preferences.Default.Set(MysteryKey,
                    Preferences.Default.Get("LastCompleteDate", -1) >= 0
                        ? Preferences.Default.Get("LastMystery", string.Empty) : string.Empty);
            return Preferences.Default.Get(MysteryKey, string.Empty);
        }
    }

    internal static bool CanRestore(DateTime today) =>
        !string.IsNullOrEmpty(SavedMystery) &&
        Preferences.Default.Get(RestoreKey, true) && !ExpireIfDue(today);

    internal static bool ExpireIfDue(DateTime today)
    {
        long scheduled = Preferences.Default.Get(ResetDateKey, 0L);
        if (scheduled == 0 || today.Date.Ticks < scheduled)
            return false;
        Preferences.Default.Set(RestoreKey, false);
        Preferences.Default.Remove(ResetDateKey);
        return true;
    }

    internal static void SelectMystery(string mystery, DateTime today)
    {
        _ = SavedMystery; // Finish legacy migration before changing the browsing selection.
        Preferences.Default.Set("LastMystery", mystery);
        int completedDay = CompletedDay(mystery);
        Preferences.Default.Set("LastDate", completedDay >= 0 ? completedDay : 1);
    }

    internal static int CompletedDay(string mystery) =>
        Preferences.Default.Get(RestoreKey, true) &&
        string.Equals(SavedMystery, mystery, StringComparison.OrdinalIgnoreCase)
            ? Preferences.Default.Get("LastCompleteDate", -1) : -1;

    internal static void SelectDay(int day, DateTime today) =>
        Preferences.Default.Set("LastDate", day);

    internal static void Complete(string mystery, int day, DateTime today)
    {
        if (string.IsNullOrWhiteSpace(mystery) || day < 0 || day > 31)
            return;
        if (!CanRestore(today) || !string.Equals(SavedMystery, mystery, StringComparison.OrdinalIgnoreCase))
            Preferences.Default.Remove(ResetDateKey);
        Preferences.Default.Set(MysteryKey, mystery);
        Preferences.Default.Set("LastMystery", mystery);
        Preferences.Default.Set("LastCompleteDate", day);
        Preferences.Default.Set("LastDate", day);
        Preferences.Default.Set(RestoreKey, true);
        if (day >= 28 && !Preferences.Default.ContainsKey(ResetDateKey))
        {
            int daysUntilSunday = (7 - (int)today.DayOfWeek) % 7;
            Preferences.Default.Set(ResetDateKey, today.Date.AddDays(daysUntilSunday).Ticks);
        }
    }
}
