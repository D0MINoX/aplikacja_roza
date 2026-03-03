namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("MyRosaryGroup", typeof(MyRosaryPage));
            Routing.RegisterRoute("RosaryMeditations", typeof(RosaryMeditationsPage));
            Routing.RegisterRoute("Prayers", typeof(PrayersPage));
            Routing.RegisterRoute("Retreat", typeof(RetreatPage));
            Routing.RegisterRoute("OrderMass", typeof(OrderMassPage));
            Routing.RegisterRoute("News", typeof(NewsPage));
            Routing.RegisterRoute("Login", typeof(LoginPage));
            Routing.RegisterRoute("Home", typeof(MainPage));
            Routing.RegisterRoute("Settings", typeof(SettingsPage));
            Routing.RegisterRoute("Messages", typeof(MessagesPage));
            Routing.RegisterRoute("SendMessage", typeof(SendMessagePage));
            Routing.RegisterRoute("ViewGroups", typeof(GroupsPage));
            Routing.RegisterRoute("FullMeditation", typeof(FullMeditationPage));
            Routing.RegisterRoute("Profile", typeof(ProfilePage));
            Routing.RegisterRoute("JoinRosary", typeof(JoinRosaryPage));
        }
    }
}
