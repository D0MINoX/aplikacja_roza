using CommunityToolkit.Maui;
using MauiApp1.Controls;
using MauiApp1.Services;
using MauiApp1.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.LocalNotification;


#if ANDROID
using Android.Content.Res;
#endif

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string baseAddress = "https://api.rosaryapi.pl";
            //#if DEBUG
            //            var handler = new HttpClientHandler();
            //            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            //            // Wybierz odpowiedni adres lokalny
            //            string baseAddress = DeviceInfo.DeviceType == DeviceType.Virtual
            //                                 ? "https://10.0.2.2:7206/"
            //                                 : "https://localhost:7206/";

            //            builder.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri(baseAddress) });
            //#endif


            // Zmiana underline color dla MyEntry
            EntryHandler.Mapper.AppendToMapping(nameof(MyEntry.UnderlineColor), (handler, view) =>
            {
#if ANDROID
                if (view is MyEntry myEntry)
                {
                    handler.PlatformView.BackgroundTintList =
                        ColorStateList.ValueOf(myEntry.UnderlineColor.ToPlatform());
                }
#endif
            });

            // Zmiana underline color dla MyEditor
            EditorHandler.Mapper.AppendToMapping(nameof(MyEditor.UnderlineColor), (handler, view) =>
            {
#if ANDROID
                if (view is MyEditor myEditor)
                {
                    handler.PlatformView.BackgroundTintList =
                        ColorStateList.ValueOf(myEditor.UnderlineColor.ToPlatform());
                }
#endif
            });

            // Zmiana underline color dla MyTimePicker
            TimePickerHandler.Mapper.AppendToMapping(nameof(MyTimePicker.UnderlineColor), (handler, view) =>
            {
#if ANDROID
                if (view is MyTimePicker myTimePicker)
                {
                    handler.PlatformView.BackgroundTintList =
                        ColorStateList.ValueOf(myTimePicker.UnderlineColor.ToPlatform());
                }
#endif
            });

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            });
#if DEBUG
            builder.Logging.AddDebug();
#endif



            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ParishService>();
            builder.Services.AddSingleton<MessagesService>();
            builder.Services.AddSingleton<MeditationsService>();
            builder.Services.AddSingleton<RosaryService>();
            builder.Services.AddSingleton<AdminService>(); 
            builder.Services.AddSingleton<NotificationsService>();


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
            builder.Services.AddTransient<RosaryAddPage>();
            builder.Services.AddTransient<MeditationAddPage>();
            builder.Services.AddTransient<ChangeUserPrivilagiesPage>();
            builder.Services.AddTransient<MessagesPage>();
            builder.Services.AddTransient<MyRosariesListPage>();
            builder.Services.AddTransient<ParishAddPage>();
            builder.Services.AddTransient<ExternalNumbersMenagementPage>();
            builder.Services.AddTransient<AgreementsMenagementPage>();
            builder.Services.AddTransient<ExternalNumbersPage>();
          
            return builder.Build();
        }
    }
}
