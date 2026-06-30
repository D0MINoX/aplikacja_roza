using MauiApp1.Services; 

namespace MauiApp1.Views;

public partial class MeditationAddPage : ContentPage
{
    private readonly AdminService _adminService;
    private bool _isWebViewLoaded = false;
    private readonly MeditationsService _meditationsService;
    public MeditationAddPage(AdminService adminService, MeditationsService meditationsService)
    {
        _adminService = adminService;
        _meditationsService = meditationsService;
        InitializeComponent();
        List<int> days = Enumerable.Range(0, 32).ToList();
        DayPicker.ItemsSource = days;
        HtmlEditorWebView.Navigated += async (s, e) =>
        {
            _isWebViewLoaded = true;
            bool isAppDarkTheme = Application.Current?.UserAppTheme == AppTheme.Dark
            || (Application.Current?.UserAppTheme == AppTheme.Unspecified && Application.Current?.RequestedTheme == AppTheme.Dark);
            await HtmlEditorWebView.EvaluateJavaScriptAsync($"setDarkMode({isAppDarkTheme.ToString().ToLower()})");
        };
        HtmlEditorWebView.Source = "editor.html";
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
      
        string htmlResult = await HtmlEditorWebView.EvaluateJavaScriptAsync("getHtml()");

        string description = System.Text.RegularExpressions.Regex.Unescape(htmlResult).Trim('"');

        // Walidacja (sprawdzamy description zamiast starego DescriptionEditor.Text)
        if (DayPicker.SelectedItem != null && MysteryPicker.SelectedItem != null && !string.IsNullOrEmpty(description) && description != "<p><br></p>")
        {
            int Date = int.Parse(DayPicker.SelectedItem.ToString());
            string Title = MysteryPicker.SelectedItem.ToString();
            string Link = null;

            var confirm = await DisplayAlertAsync("INFO", "Czy na pewno chcesz zmienić rozważanie?", "TAK", "NIE");
            if (confirm)
            {
               
                // Wysyłamy sformatowany kod HTML prosto do bazy danych!
                bool isSuccess = await _adminService.ModifyMeditationAsync(Title, description, Date, Link);
                if (isSuccess)
                {
                    await DisplayAlertAsync("INFO", "Zmieniono treść rozważania", "OK");
                }
                else
                {
                    await DisplayAlertAsync("Błąd", "Błąd dodawania rozważania", "OK");
                }
            }
        }
    }
    private async void OnDetailChanged(object sender, EventArgs e)
    {
        
        if (!_isWebViewLoaded) return;

        if (MysteryPicker.SelectedItem != null && DayPicker.SelectedItem != null)
        {
            int Date = int.Parse(DayPicker.SelectedItem.ToString()!);
            string Title = MysteryPicker.SelectedItem.ToString()!;

            var data = await _meditationsService.GetMeditationData(Date, Title);
            if (data != null)
            {
                string safeHtml = System.Web.HttpUtility.JavaScriptStringEncode(data.Content);
                await HtmlEditorWebView.EvaluateJavaScriptAsync($"setHtml('{safeHtml}')");

                
            }
            else
            {
                await HtmlEditorWebView.EvaluateJavaScriptAsync("setHtml('')");
            }
        }
    }
}