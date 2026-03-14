using MauiApp1.Services;
using MauiApp1.Views;
using Microsoft.Extensions.Logging;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string baseAddress = "https://api.rosaryapi.pl";


            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<MeditationsService>();
            builder.Services.AddSingleton<RosaryService>();
            builder.Services.AddSingleton<AdminService>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<MyRosaryPage>();
            builder.Services.AddTransient<RosaryMeditationsPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<JoinRosaryPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AdminPage>();
            builder.Services.AddTransient<RosaryMenagementPage>();
            builder.Services.AddTransient<UserVerificationPage>();
            builder.Services.AddTransient<AdminRosariesPage>();
            return builder.Build();
        }
    }
}
