namespace MauiApp1.Services;

internal static class RosaryCycle
{
    private const int FirstClosingDay = 28;
    private const string RestoreKey = "RestoreCalendarOnStart";
    private const string ResetDateKey = "RosaryCycleResetDate";

    internal static bool CanRestore(DateTime today)
    {
        bool legacyRestore = Preferences.Default.Get("LastDate", 0) < FirstClosingDay;
        return Preferences.Default.Get(RestoreKey, legacyRestore)
            && !ExpireIfDue(today);
    }

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
        string previous = Preferences.Default.Get("LastMystery", string.Empty);
        if (!CanRestore(today) || !string.Equals(previous, mystery, StringComparison.OrdinalIgnoreCase))
        {
            Preferences.Default.Set("LastCompleteDate", -1);
            Preferences.Default.Set("LastDate", 1);
            Preferences.Default.Remove(ResetDateKey);
        }

        Preferences.Default.Set("LastMystery", mystery);
        Preferences.Default.Set(RestoreKey, true);
    }

    internal static void SelectDay(int day, DateTime today)
    {
        bool active = CanRestore(today);
        Preferences.Default.Set("LastDate", day);
        if (!active)
            return;

        // Persist the legacy default before a late day changes LastDate.
        Preferences.Default.Set(RestoreKey, true);
        if (day >= FirstClosingDay && !Preferences.Default.ContainsKey(ResetDateKey))
        {
            int daysUntilSunday = (7 - (int)today.DayOfWeek) % 7;
            Preferences.Default.Set(ResetDateKey, today.Date.AddDays(daysUntilSunday).Ticks);
        }
    }
}
