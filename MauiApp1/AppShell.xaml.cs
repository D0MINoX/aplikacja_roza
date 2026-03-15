using MauiApp1.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("MyRosaryGroup", typeof(MyRosaryPage));
            Routing.RegisterRoute("RosaryMeditations", typeof(RosaryMeditationsPage));

            Routing.RegisterRoute("Login", typeof(LoginPage));

            Routing.RegisterRoute("Settings", typeof(SettingsPage));

            Routing.RegisterRoute("FullMeditation", typeof(FullMeditationPage));
            Routing.RegisterRoute("Profile", typeof(ProfilePage));
            Routing.RegisterRoute("JoinRosary", typeof(JoinRosaryPage));
            Routing.RegisterRoute("Register", typeof(RegisterPage));
            Routing.RegisterRoute("AdminPage", typeof(AdminPage));
            Routing.RegisterRoute("RosaryMenagement", typeof(RosaryMenagementPage));
            Routing.RegisterRoute("UserVerification", typeof(UserVerificationPage));
            Routing.RegisterRoute("AdminRosaries", typeof(AdminRosariesPage));
            Routing.RegisterRoute("RosaryAdd", typeof(RosaryAddPage));
            Routing.RegisterRoute("MeditationAdd", typeof(MeditationAddPage));
            Routing.RegisterRoute("ChangeUserPrivilagies", typeof(ChangeUserPrivilagiesPage));
        }
    }
}
