using MauiApp1.Services;

namespace MauiApp1;

public partial class SettingsPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly NotificationsService _notificationsService;
    public SettingsPage(AuthService authService,NotificationsService notificationsService)
    {
        _authService = authService;
        _notificationsService = notificationsService;

        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        IsLogged();

        DownloadSwitch.Toggled -= OnDownloadToggled;
        DownloadSwitch.IsToggled = Preferences.Default.Get("AutoDownloadMeditations", false);
        DownloadSwitch.Toggled += OnDownloadToggled;

        ReminderSwitch.IsToggled = Preferences.Default.Get("RemindersEnabled", false);

        ReminderTimePicker.SetValue(TimePicker.TimeProperty, TimeSpan.Parse(Preferences.Default.Get("ReminderTime", "20:00:00")));
    }

    private async void IsLogged()
    {
        bool hasToken = await _authService.CheckAndSetTokenAsync();

        if (hasToken)
        {
            LogIn.IsVisible = false;
            LogOut.IsVisible = true;
            Profile.IsVisible = true;
        }
        else
        {
            LogIn.IsVisible = true;
            LogOut.IsVisible = false;
            Profile.IsVisible = false;
        }
    }

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Login");
    }

    private async void Logout_Tapped(object sender, TappedEventArgs e)
    {
        bool IsLoggedout = await _authService.Logout();
        if (IsLoggedout)
        {
            await DisplayAlertAsync("Informacja", "Konto zostało pomyślnie wylogowane", "OK");
        }
        else
        {
            await DisplayAlertAsync("BŁĄD", "Wylogowanie nie powiodło się, spróbuj ponownie", "OK");
        }
        IsLogged();
    }

    private async void Profile_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Profile");
    }

    private void ThemeTapped(object sender, TappedEventArgs e)
    {
        ThemeManager.SetMainTheme();
    }

    private async void OnDownloadToggled(object sender, ToggledEventArgs e)
    {
        bool isAllowed = e.Value;
        Preferences.Default.Set("AutoDownloadMeditations", isAllowed);

        if (isAllowed)
        {

            await DisplayAlertAsync("Offline", "Aplikacja pobierze teraz rozważania na cały miesiąc.", "OK");
        }

    }

    private async void OnRemindersToggled(object sender, ToggledEventArgs e)
    {
        bool isAllowed = e.Value;
        Preferences.Default.Set("RemindersEnabled", isAllowed);
        if (isAllowed)
        {
            ReminderTimeLayout.IsVisible=true;
            await _notificationsService.ScheduleWeeklyReminders();
        }
        else
        {
            ReminderTimeLayout.IsVisible=false;
            await _notificationsService.CancelAllReminders();
        }
    }

    private async void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
        await _notificationsService.CancelAllReminders();
        Preferences.Default.Set("ReminderTime", e.NewTime.ToString());
        await _notificationsService.ScheduleWeeklyReminders();
    }

    private async void ReportBug_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ReportBug");
    }
}