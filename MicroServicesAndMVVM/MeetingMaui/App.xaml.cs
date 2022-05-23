namespace MeetingMaui
{
    //#if DEBUG
    //    [Android.App.Application(Debuggable = true, NetworkSecurityConfig = "@xml/network_security_config")]
    //#else
    //    [Application(Debuggable = false)]
    //#endif
    //    public partial class MainApplication : Android.App.Application
    //    {
    //    }


    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }


}