using MauiApp1.Services;
using Microsoft.Extensions.Logging;
using MauiApp1.Views;

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
#if DEBUG
         
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            string baseAddress = "";
#if WINDOWS
               baseAddress= "https://localhost:7206";
#elif ANDROID
            baseAddress = "https://10.0.2.2:7206";
#endif
            builder.Services.AddSingleton(new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress)
            });
        
#endif
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
