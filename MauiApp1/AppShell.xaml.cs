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
        }
    }
}
