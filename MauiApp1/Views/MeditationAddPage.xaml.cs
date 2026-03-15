using MauiApp1.Services; 

namespace MauiApp1.Views;

public partial class MeditationAddPage : ContentPage
{
    private readonly AdminService _adminService;
    private readonly MeditationsService _meditationsService;
    public MeditationAddPage(AdminService adminService, MeditationsService meditationsService)
    {
        _adminService = adminService;
        _meditationsService = meditationsService;
        InitializeComponent();
        List<int> days = Enumerable.Range(1, 31).ToList();
        DayPicker.ItemsSource = days;
        
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        if (DayPicker.SelectedItem != null && MysteryPicker.SelectedItem != null && DescriptionEditor.Text != null)
        {
            int Date = int.Parse(DayPicker.SelectedItem.ToString());
            string Title = MysteryPicker.SelectedItem.ToString();
            string description = DescriptionEditor.Text;
            var confirm = await DisplayAlertAsync("INFO", "Czy napewno chcesz zmienić rozważanie?", "TAK", "NIE");
            if (confirm)
            {
                bool isSuccess = await _adminService.ModifyMeditationAsync(Title,description,Date);
                if (isSuccess)
                {
                    await DisplayAlertAsync("INFO", "Zmieniono treść rozważania", "OK");


                }
                else
                {

                    await DisplayAlertAsync("Błąd","Błąd dodawania rozważania" , "OK");

                    
                }
            }
        }
        
    }
    private async void OnDetailChanged(object sender, EventArgs e)
    {
        if(MysteryPicker.SelectedItem!=null && DayPicker.SelectedItem != null)
        {

            int Date = int.Parse(DayPicker.SelectedItem.ToString());
            string Title = MysteryPicker.SelectedItem.ToString();
            string description =await _meditationsService.GetOnlyDescription(Date, Title);
            DescriptionEditor.Text = description;
        }
    }
}